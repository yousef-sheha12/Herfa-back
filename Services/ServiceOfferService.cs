using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using BCrypt.Net;
using Herfa_back.DTOs.Offer;
using Herfa_back.Helpers;
using Herfa_back.Interfaces.IRepository;
using Herfa_back.Interfaces.IService;
using Herfa_back.Models;
using Herfa_back.Models.Enums;
using Microsoft.EntityFrameworkCore;

using Herfa_back.Data;

namespace Herfa_back.Services
{
    public class ServiceOfferService : IServiceOfferService
    {
        private readonly AppDbContext _context;
        private readonly IServiceOfferRepository _offerRepository;
        private readonly IServiceRequestRepository _requestRepository;
        private readonly IArtisanRepository _artisanRepository;
        private readonly INotificationService _notificationService;
        private readonly IJobService _jobService;

        public ServiceOfferService(
            AppDbContext context,
            IServiceOfferRepository offerRepository,
            IServiceRequestRepository requestRepository,
            IArtisanRepository artisanRepository,
            INotificationService notificationService,
            IJobService jobService)
        {
            _context = context;
            _offerRepository = offerRepository;
            _requestRepository = requestRepository;
            _artisanRepository = artisanRepository;
            _notificationService = notificationService;
            _jobService = jobService;
        }

        public async Task<OfferDto> CreateOfferAsync(CreateOfferDto dto, int requestId, int artisanUserId)
        {

            var artisan = await _artisanRepository.GetByUserIdAsync(artisanUserId);
            if (artisan == null) throw new Exception("Artisan profile not found.");

            var offer = new ServiceOffer
            {
                ServiceRequestId = requestId,
                ArtisanId = artisan.Id,
                Message = dto.Message,
                Price = dto.Price,
                Status = ServiceOfferStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _offerRepository.AddAsync(offer);
            await _offerRepository.SaveChangesAsync();

            // Notify the client
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request != null && artisan != null)
            {
                await _notificationService.SendAsync(
                    userId: request.ClientId,
                    title: "New Offer Received",
                    message: $"Offer received from {artisan.User.Username} for '{request.Title}'.",
                    type: NotificationType.NewOffer,
                    referenceId: offer.Id,
                    price: dto.Price,
                    comment: dto.Message
                );
            }

            return new OfferDto
            {
                Id = offer.Id,
                ArtisanId = offer.ArtisanId,
                Price = offer.Price,
                Message = offer.Message,
                Status = offer.Status.ToString(),
                CreatedAt = offer.CreatedAt
            };
        }

        public async Task<IEnumerable<OfferDto>> GetOffersByRequestAsync(int requestId)
        {
            var offers = await _offerRepository.GetByRequestIdAsync(requestId);

            return offers.Select(offer => new OfferDto
            {
                Id = offer.Id,
                ArtisanId = offer.ArtisanId,
                ArtisanName = offer.Artisan.User.Username,
                Price = offer.Price,
                Message = offer.Message,
                Status = offer.Status.ToString(),
                CreatedAt = offer.CreatedAt
            });
        }

        public async Task<int> AcceptOfferAsync(int offerId)
        {
            var offer = await _offerRepository.GetByIdAsync(offerId);
            if (offer == null)
                throw new Exception("Offer not found.");

            var request = await _requestRepository.GetByIdAsync(offer.ServiceRequestId);
            if (request == null)
                throw new Exception("Service Request not found.");

            var strategy = _context.Database.CreateExecutionStrategy();
            int createdJobId = 0;

            await strategy.ExecuteAsync(async () =>
            {
                using var tx = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. Accept the offer
                    offer.Status = ServiceOfferStatus.Accepted;
                    offer.UpdatedAt = DateTime.UtcNow;
                    await _offerRepository.UpdateAsync(offer);

                    // 2. Reject all other pending offers
                    var allOffers = await _offerRepository.GetByRequestIdAsync(request.Id);
                    foreach (var otherOffer in allOffers.Where(o => o.Id != offerId && o.Status == ServiceOfferStatus.Pending))
                    {
                        otherOffer.Status = ServiceOfferStatus.Rejected;
                        otherOffer.UpdatedAt = DateTime.UtcNow;
                        await _offerRepository.UpdateAsync(otherOffer);

                        // Notify rejected artisans
                        await _notificationService.SendAsync(
                            userId: otherOffer.Artisan.UserId,
                            title: "Offer Rejected",
                            message: $"Another offer was chosen for '{request.Title}'. Your offer has been rejected.",
                            type: NotificationType.OfferRejected,
                            referenceId: otherOffer.Id
                        );
                    }

                    // 3. Update Request status
                    request.Status = ServiceRequestStatus.InProgress;
                    request.UpdatedAt = DateTime.UtcNow;
                    await _requestRepository.UpdateAsync(request);

                    // 4. Create Job
                    var jobDto = await _jobService.CreateJobAsync(request.Id, offer.ArtisanId, request.ClientId);
                    createdJobId = jobDto.Id;

                    // 5. Notify Accepted Artisan
                    var artisan = await _artisanRepository.GetByIdAsync(offer.ArtisanId);
                    if (artisan != null)
                    {
                        await _notificationService.SendAsync(
                            userId: artisan.UserId,
                            title: "Offer Accepted",
                            message: $"Congratulations! Your offer for '{request.Title}' has been accepted and the Job is now in progress.",
                            type: NotificationType.OfferAccepted,
                            referenceId: offer.Id
                        );
                    }

                    await _context.SaveChangesAsync();
                    await tx.CommitAsync();
                }
                catch (Exception)
                {
                    await tx.RollbackAsync();
                    throw;
                }
            });

            return createdJobId;
        }

        public async Task RejectOfferAsync(int offerId)
        {
            var offer = await _offerRepository.GetByIdAsync(offerId);

            if (offer == null)
                throw new Exception("Offer not found.");

            offer.Status = ServiceOfferStatus.Rejected;
            offer.UpdatedAt = DateTime.UtcNow;

            await _offerRepository.UpdateAsync(offer);
            await _offerRepository.SaveChangesAsync();

            // Notify the artisan
            var request = await _requestRepository.GetByIdAsync(offer.ServiceRequestId);
            var artisan = await _artisanRepository.GetByIdAsync(offer.ArtisanId);
            if (request != null && artisan != null)
            {
                await _notificationService.SendAsync(
                    userId: artisan.UserId,
                    title: "Offer Rejected",
                    message: $"Your offer for '{request.Title}' has been rejected.",
                    type: NotificationType.OfferRejected,
                    referenceId: offer.Id
                );
            }
        }
    }
}

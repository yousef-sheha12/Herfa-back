using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using BCrypt.Net;
using Herfa_back.DTOs.Offer;
using Herfa_back.Helpers;
using Herfa_back.Interfaces.IRepository;
using Herfa_back.Interfaces.IService;
using Herfa_back.Models;

namespace Herfa_back.Services
{
    public class ServiceOfferService : IServiceOfferService
    {
        private readonly IServiceOfferRepository _offerRepository;

        public ServiceOfferService(IServiceOfferRepository offerRepository)
        {
            _offerRepository = offerRepository;
        }

        public async Task<OfferDto> CreateOfferAsync(CreateOfferDto dto, int requestId, int artisanId)
        {
            var existingOffer = await _offerRepository
                .GetByRequestAndArtisanAsync(requestId, artisanId);

            if (existingOffer != null)
                throw new Exception("You have already submitted an offer for this request.");

            var offer = new ServiceOffer
            {
                ServiceRequestId = requestId,
                ArtisanId = artisanId,
                Message = dto.Message,
                Price = dto.Price,
                Status = ServiceOfferStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _offerRepository.AddAsync(offer);
            await _offerRepository.SaveChangesAsync();

            return new OfferDto
            {
                Id = offer.Id,
                ArtisanId = offer.ArtisanId,
                Price = offer.Price,
                Message = offer.Message,
                Status = offer.Status,
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
                ArtisanName = offer.Artisan.User.FullName,
                Price = offer.Price,
                Message = offer.Message,
                Status = offer.Status,
                CreatedAt = offer.CreatedAt
            });
        }

        public async Task AcceptOfferAsync(int offerId)
        {
            var offer = await _offerRepository.GetByIdAsync(offerId);

            if (offer == null)
                throw new Exception("Offer not found.");

            offer.Status = ServiceOfferStatus.Accepted;
            offer.UpdatedAt = DateTime.UtcNow;

            await _offerRepository.UpdateAsync(offer);
            await _offerRepository.SaveChangesAsync();
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
        }
    }
}

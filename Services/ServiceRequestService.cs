using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using BCrypt.Net;
using Herfa_back.DTOs.Request;
using Herfa_back.Helpers;
using Herfa_back.Interfaces.IRepository;
using Herfa_back.Interfaces.IService;
using Herfa_back.Models;
using Herfa_back.Models.Enums;
using Herfa_back.DTOs.Offer;

namespace Herfa_back.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _requestRepository;
        private readonly IArtisanRepository _artisanRepository;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;

        public ServiceRequestService(
            IServiceRequestRepository requestRepository,
            IArtisanRepository artisanRepository,
            INotificationService notificationService,
            IUserRepository userRepository)
        {
            _requestRepository = requestRepository;
            _artisanRepository = artisanRepository;
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        public async Task<CreateServiceRequestResponseDto> CreateRequestAsync(CreateServiceRequestDto dto, int clientId)
        {
            var serviceRequest = new ServiceRequest
            {
                CategoryId = dto.CategoryId,
                ClientId = clientId,
                Title = dto.Title,
                Description = dto.Description,
                Address = dto.Address,
                ImageUrl = dto.ImageUrl,
                Status = ServiceRequestStatus.Open,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _requestRepository.AddAsync(serviceRequest);
            await _requestRepository.SaveChangesAsync();

            // Notify all artisans in the same category
            var artisans = await _artisanRepository.getByCategory(dto.CategoryId);
            foreach (var artisan in artisans)
            {
                await _notificationService.SendAsync(
                    userId: artisan.UserId,
                    title: "New Service Request",
                    message: $"A new request (ID: {serviceRequest.Id}) '{serviceRequest.Title}' has been posted in your category.",
                    type: NotificationType.NewRequest,
                    referenceId: serviceRequest.Id
                );
            }

            var client = await _userRepository.GetByIdAsync(clientId);

            return new CreateServiceRequestResponseDto
            {
                ClientName = client?.Username ?? string.Empty,
                CategoryId = serviceRequest.CategoryId,
                Title = serviceRequest.Title,
                Description = serviceRequest.Description,
                Address = serviceRequest.Address,
                ImageUrl = serviceRequest.ImageUrl
            };
        }

        public async Task<IEnumerable<ServiceRequestListDto>> GetAllRequestsAsync()
        {
            var requests = await _requestRepository.GetAllAsync();

            return requests.Select(request => new ServiceRequestListDto
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                Address = request.Address,
                Status = request.Status.ToString(),
                CreatedAt = request.CreatedAt,
                CategoryName = request.Category.Name
            });
        }

        public async Task<ServiceRequestDetailsDto?> GetRequestByIdAsync(int id)
        {
            var request = await _requestRepository.GetByIdAsync(id);

            if (request == null)
                return null;

            return new ServiceRequestDetailsDto
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                Address = request.Address,
                ImageUrl = request.ImageUrl,
                Status = request.Status.ToString(),
                CreatedAt = request.CreatedAt,
                CategoryName = request.Category.Name,

                Offers = request.Offers.Select(offer => new OfferDto
                {
                    Id = offer.Id,
                    ArtisanId = offer.ArtisanId,
                    ArtisanName = offer.Artisan.User.Username,
                    Price = offer.Price,
                    Message = offer.Message,
                    Status = offer.Status.ToString(),
                    CreatedAt = offer.CreatedAt
                }).ToList()
            };
        }

        public async Task CancelRequestAsync(int id)
        {
            var request = await _requestRepository.GetByIdAsync(id);

            if (request == null)
                throw new Exception("Request not found.");

            request.Status = ServiceRequestStatus.Cancelled;
            request.UpdatedAt = DateTime.UtcNow;

            await _requestRepository.UpdateAsync(request);
            await _requestRepository.SaveChangesAsync();
        }
    }
}
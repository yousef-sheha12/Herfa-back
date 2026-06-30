using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using BCrypt.Net;
using Herfa_back.DTOs.Request;
using Herfa_back.Helpers;
using Herfa_back.Interfaces.IRepository;
using Herfa_back.Interfaces.IService;
using Herfa_back.Models;

namespace Herfa_back.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _requestRepository;

        public ServiceRequestService(IServiceRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<ServiceRequestDetailsDto> CreateRequestAsync(CreateServiceRequestDto dto, int clientId)
        {
            var serviceRequest = new ServiceRequest
            {
                CategoryId = dto.CategoryId,
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

            return new ServiceRequestDetailsDto
            {
                Id = serviceRequest.Id,
                Title = serviceRequest.Title,
                Description = serviceRequest.Description,
                Address = serviceRequest.Address,
                ImageUrl = serviceRequest.ImageUrl,
                Status = serviceRequest.Status,
                CreatedAt = serviceRequest.CreatedAt,
                CategoryName = "",
                Offers = new List<OfferDto>()
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
                Status = request.Status,
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
                Status = request.Status,
                CreatedAt = request.CreatedAt,
                CategoryName = request.Category.Name,

                Offers = request.Offers.Select(offer => new OfferDto
                {
                    Id = offer.Id,
                    ArtisanId = offer.ArtisanId,
                    ArtisanName = offer.Artisan.User.FullName,
                    Price = offer.Price,
                    Message = offer.Message,
                    Status = offer.Status,
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
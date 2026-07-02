using Herfa_back.DTOs.Request;
using Herfa_back.Models;

namespace Herfa_back.Interfaces.IService
{
        public interface IServiceRequestService
        {
            Task<CreateServiceRequestResponseDto> CreateRequestAsync(CreateServiceRequestDto dto, int clientId);

            // Get all service requests
            Task<IEnumerable<ServiceRequestListDto>> GetAllRequestsAsync();

            // Get request details by id
            Task<ServiceRequestDetailsDto?> GetRequestByIdAsync(int id);

            // Cancel service request
            Task CancelRequestAsync(int id);
        }
    }

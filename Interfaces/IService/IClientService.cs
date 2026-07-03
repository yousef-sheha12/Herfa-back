using Herfa_back.DTOs.Client;

namespace Herfa_back.Interfaces.IService
{
    public interface IClientService
    {
        Task<ClientDashboardDto> GetDashboardStatsAsync(int clientId);
        Task<ClientProfileDto?> GetProfileAsync(int clientId);
        Task<bool> UpdateProfileAsync(int clientId, UpdateClientProfileDto dto);
    }
}

using Herfa_back.Data;
using Herfa_back.DTOs.Client;
using Herfa_back.Interfaces.IService;
using Herfa_back.Models;
using Herfa_back.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Herfa_back.Services
{
    public class ClientService : IClientService
    {
        private readonly AppDbContext _context;

        public ClientService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ClientDashboardDto> GetDashboardStatsAsync(int clientId)
        {
            var totalRequests = await _context.ServiceRequests
                .CountAsync(r => r.ClientId == clientId && !r.IsDeleted);

            var activeRequests = await _context.ServiceRequests
                .CountAsync(r => r.ClientId == clientId && !r.IsDeleted &&
                                 (r.Status == ServiceRequestStatus.Open || r.Status == ServiceRequestStatus.InProgress));

            var completedJobs = await _context.Jobs
                .Include(j => j.ServiceRequest)
                .ThenInclude(r => r.Offers)
                .Where(j => j.ClientId == clientId && j.Status == JobStatus.Completed && !j.IsDeleted)
                .ToListAsync();

            decimal totalSpent = 0;
            foreach (var job in completedJobs)
            {
                var acceptedOffer = job.ServiceRequest.Offers
                    .FirstOrDefault(o => o.ArtisanId == job.ArtisanId && o.Status == ServiceOfferStatus.Accepted);
                
                if (acceptedOffer != null)
                {
                    totalSpent += acceptedOffer.Price;
                }
            }

            return new ClientDashboardDto
            {
                TotalRequests = totalRequests,
                ActiveRequests = activeRequests,
                CompletedJobs = completedJobs.Count,
                TotalSpent = totalSpent
            };
        }

        public async Task<ClientProfileDto?> GetProfileAsync(int clientId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == clientId && !u.IsDeleted);
            if (user == null) return null;

            return new ClientProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }

        public async Task<bool> UpdateProfileAsync(int clientId, UpdateClientProfileDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == clientId && !u.IsDeleted);
            if (user == null) return false;

            user.Username = dto.Username;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}

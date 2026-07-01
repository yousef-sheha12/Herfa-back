using Herfa_back.Data;
using Herfa_back.Interfaces.IRepository;
using Herfa_back.Models;
using Herfa_back.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Herfa_back.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly AppDbContext _context;

        public JobRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Job>> GetAllAsync()
        {
            return await _context.Jobs
                .Include(j => j.Client)
                .Include(j => j.Artisan).ThenInclude(a => a.User)
                .Include(j => j.ServiceRequest)
                .Where(j => !j.IsDeleted)
                .ToListAsync();
        }

        public async Task<Job?> GetByIdAsync(int id)
        {
            return await _context.Jobs
                .Include(j => j.Client)
                .Include(j => j.Artisan).ThenInclude(a => a.User)
                .Include(j => j.ServiceRequest)
                .FirstOrDefaultAsync(j => j.Id == id && !j.IsDeleted);
        }

        public async Task<List<Job>> GetByArtisanAsync(int artisanId)
        {
            return await _context.Jobs
                .Include(j => j.Client)
                .Include(j => j.Artisan).ThenInclude(a => a.User)
                .Include(j => j.ServiceRequest)
                .Where(j => j.ArtisanId == artisanId && !j.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Job>> GetByClientAsync(int clientId)
        {
            return await _context.Jobs
                .Include(j => j.Client)
                .Include(j => j.Artisan).ThenInclude(a => a.User)
                .Include(j => j.ServiceRequest)
                .Where(j => j.ClientId == clientId && !j.IsDeleted)
                .ToListAsync();
        }

        public async Task AddAsync(Job job)
        {
            await _context.Jobs.AddAsync(job);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Job job)
        {
            job.UpdatedAt = DateTime.UtcNow;
            _context.Jobs.Update(job);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateServiceRequestStatusAsync(int serviceRequestId, ServiceRequestStatus status)
        {
            var request = await _context.ServiceRequests
                .FirstOrDefaultAsync(sr => sr.Id == serviceRequestId);

            if (request != null)
            {
                request.Status = status;
                request.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
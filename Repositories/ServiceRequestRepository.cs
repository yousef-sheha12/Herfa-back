using Herfa_back.Data;
using Herfa_back.Interfaces.IRepository;
using Herfa_back.Models;
using Microsoft.EntityFrameworkCore;

namespace Herfa_back.Repositories
{
    public class ServiceRequestRepository :IServiceRequestRepository
    {
        private readonly AppDbContext _context;
        public ServiceRequestRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ServiceRequest> AddAsync(ServiceRequest serviceRequest)
        {
            await _context.ServiceRequests.AddAsync(serviceRequest);

            return serviceRequest;
        }
        public async Task<ServiceRequest?> GetByIdAsync(int id)
        {
            return await _context.ServiceRequests
                .Include(x => x.Client)
                .Include(x => x.Category)
                .Include(x => x.Offers)
                   .ThenInclude(o => o.Artisan)
                     .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    !x.IsDeleted);
        }
        public async Task<IEnumerable<ServiceRequest>> GetAllAsync()
        {
            return await _context.ServiceRequests
                .Include(x => x.Category)
                .Include(x => x.Client)
                .Where(x => !x.IsDeleted)
                .ToListAsync();
        }
        public Task UpdateAsync(ServiceRequest serviceRequest)
        {
            _context.ServiceRequests.Update(serviceRequest);

            return Task.CompletedTask;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

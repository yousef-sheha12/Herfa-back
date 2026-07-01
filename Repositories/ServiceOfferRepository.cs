using Herfa_back.Data;
using Herfa_back.Interfaces.IRepository;
using Herfa_back.Models;
using Microsoft.EntityFrameworkCore;

namespace Herfa_back.Repositories
{
        public class ServiceOfferRepository : IServiceOfferRepository
        {
            private readonly AppDbContext _context;

            public ServiceOfferRepository(AppDbContext context)
            {
                _context = context;
            }
        public async Task<ServiceOffer> AddAsync(ServiceOffer serviceOffer)//POST /requests/{id}/offers
        {
            await _context.ServiceOffers.AddAsync(serviceOffer);

            return serviceOffer;
        }
        public async Task<ServiceOffer?> GetByIdAsync(int id)
        {
            return await _context.ServiceOffers
                .Include(x => x.Artisan)
                    .ThenInclude(x => x.User)
                .Include(x => x.ServiceRequest)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    !x.IsDeleted);
        }
        public async Task<IEnumerable<ServiceOffer>> GetByRequestIdAsync(int requestId) //GET /requests/{id}/offers
        {
            return await _context.ServiceOffers
                .Include(x => x.Artisan)
                    .ThenInclude(x => x.User)
                .Where(x =>
                    x.ServiceRequestId == requestId &&
                    !x.IsDeleted)
                .ToListAsync();
        }
        public async Task<ServiceOffer?> GetByRequestAndArtisanAsync(int requestId, int artisanId)
        {
            return await _context.ServiceOffers
                .FirstOrDefaultAsync(x =>
                    x.ServiceRequestId == requestId &&
                    x.ArtisanId == artisanId &&
                    !x.IsDeleted);
        }
        public Task UpdateAsync(ServiceOffer serviceOffer)//PATCH /offers/{id}/reject
        {
            _context.ServiceOffers.Update(serviceOffer);

            return Task.CompletedTask;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
        

}


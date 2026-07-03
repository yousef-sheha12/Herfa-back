using Herfa_back.Data;
using Herfa_back.Interfaces.IRepository;
using Herfa_back.Models;
using Microsoft.EntityFrameworkCore;

namespace Herfa_back.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        public async Task<Review?> GetByJobIdAsync(int jobId)
        {
            return await _context.Reviews
                .Include(r => r.Client)
                .FirstOrDefaultAsync(r => r.JobId == jobId);
        }

        public async Task<List<Review>> GetByArtisanIdAsync(int artisanId)
        {
            return await _context.Reviews
                .Include(r => r.Client)
                .Where(r => r.ArtisanId == artisanId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

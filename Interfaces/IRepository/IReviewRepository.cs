using Herfa_back.Models;

namespace Herfa_back.Interfaces.IRepository
{
    public interface IReviewRepository
    {
        Task<Review?> GetByJobIdAsync(int jobId);
        Task<List<Review>> GetByArtisanIdAsync(int artisanId);
        Task AddAsync(Review review);
        Task SaveChangesAsync();
    }
}

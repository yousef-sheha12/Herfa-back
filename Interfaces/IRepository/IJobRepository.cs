using Herfa_back.Models;

namespace Herfa_back.Interfaces.IRepository
{
    public interface IJobRepository
    {
        Task<List<Job>> GetAllAsync();
        Task<Job?> GetByIdAsync(int id);
        Task<List<Job>> GetByArtisanAsync(int artisanId);
        Task<List<Job>> GetByClientAsync(int clientId);
        Task AddAsync(Job job);
        Task UpdateAsync(Job job);
    }
}
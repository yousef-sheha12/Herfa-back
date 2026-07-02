using Herfa_back.Models;

namespace Herfa_back.Interfaces.IRepository
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
        Task<Category> AddAsync(Category category);
    }
}

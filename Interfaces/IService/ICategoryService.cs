using Herfa_back.DTOs.Category;
using Herfa_back.Models;

namespace Herfa_back.Interfaces.IService
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync();
        Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
        Task<CategoryDto> AddAsync(CreateCategoryDto category, CancellationToken cancellationToken = default);
    }
}

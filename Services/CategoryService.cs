using Herfa_back.DTOs.Category;
using Herfa_back.Interfaces.IService;
using Herfa_back.Models;
using Herfa_back.Repositories;
namespace Herfa_back.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly CategoryRepository _repo;

        public CategoryService(CategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<CategoryDto> AddAsync(CreateCategoryDto dto)
        {
            var exists = await _repo.ExistsAsync(dto.Name);
            if (exists)
                throw new InvalidOperationException("Category already exists.");

            var category = new Category
            {
                Name = dto.Name,
                IconUrl = dto.IconUrl,
                Description = dto.Description
            };

            var created = await _repo.AddAsync(category);

            return new CategoryDto
            {
                Id = created.Id,
                Name = created.Name,
                IconUrl = created.IconUrl,
                Description = created.Description
            };
        }

        public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;

            // تمرير الـ token للـ repo
            return await _repo.ExistsAsync(name.Trim(), cancellationToken);
        }
        
        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var categories = await _repo.GetAllAsync();

            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                IconUrl = c.IconUrl,
                Description = c.Description
            }).ToList();
        }
    }
}

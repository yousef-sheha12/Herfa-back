using Herfa_back.Data;
using Herfa_back.Interfaces.IRepository;
using Herfa_back.Models;
using Herfa_back.DTOs.Category;
using Microsoft.EntityFrameworkCore;

namespace Herfa_back.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Category> AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .ToListAsync();
        }
    }
}

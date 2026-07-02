using Herfa_back.Data;
using Herfa_back.Interfaces.IRepository;
using Herfa_back.Models;
using Microsoft.EntityFrameworkCore;

namespace Herfa_back.Repositories
{
    public class ArtisanRepository : IArtisanRepository
    {
        private readonly AppDbContext _context;

        public ArtisanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ArtisanProfile artisan)
        {
            await _context.ArtisanProfiles.AddAsync(artisan);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ArtisanProfile>> GetAllAsync()
        {
            return await _context.ArtisanProfiles
                .Include(a => a.Category)
                .ToListAsync();
        }

        public async Task<List<ArtisanProfile>> getByCategory(int categoryId)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Invalid category ID.");

            return await _context.ArtisanProfiles
                .Where(a => a.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<List<ArtisanProfile>> getByCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be empty.");

            return await _context.ArtisanProfiles
                .Where(a => a.City.ToLower() == city.ToLower())
                .ToListAsync();
        }

        public async Task<ArtisanProfile?> GetByIdAsync(int id)
        {
            return await _context.ArtisanProfiles
                .Include(a => a.Category)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<ArtisanProfile?> GetByUserIdAsync(int userId)
        {
            return await _context.ArtisanProfiles
                .Include(a => a.Category)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.UserId == userId);
        }

        public async Task<bool> NationalIdExistsAsync(string nationalId)
        {
            return await _context.ArtisanProfiles
                .AnyAsync(a => a.NationalId == nationalId);
        }

        public async Task RecalculateRatingAsync(int artisanId)
        {
            var artisan = await _context.ArtisanProfiles
                .Include(a => a.Reviews)
                .FirstOrDefaultAsync(a => a.Id == artisanId);

            if (artisan == null)
                throw new KeyNotFoundException($"Artisan with ID {artisanId} not found.");

            if (artisan.Reviews.Any())
            {
                artisan.Rating = (float)artisan.Reviews.Average(r => r.Rating);
                artisan.TotalReviews = artisan.Reviews.Count;
            }
            else
            {
                //  لو مفيش reviews، بنصفر التقييم
                artisan.Rating = 0;
                artisan.TotalReviews = 0;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ToggleAvailabilityAsync(int artisanId)
        {
            var artisan = await _context.ArtisanProfiles.FindAsync(artisanId);

            if (artisan == null)
                return false; // مش موجود

            artisan.IsAvailable = !artisan.IsAvailable;
            await _context.SaveChangesAsync();
            return true; // اتعمل
        }

        public async Task UpdateAsync(ArtisanProfile artisan)
        {
            _context.ArtisanProfiles.Update(artisan);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> VerifyAsync(int id)
        {
            var artisan = await _context.ArtisanProfiles.FindAsync(id);
            if (artisan == null) return false;

            artisan.IsVerified = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

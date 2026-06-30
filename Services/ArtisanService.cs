using Herfa_back.DTOs.Artisan;
using Herfa_back.Interfaces.IService;
using Herfa_back.Models;
using Herfa_back.Repositories;

namespace Herfa_back.Services
{
    public class ArtisanService : IArtisanService
    {
        private readonly ArtisanRepository _Repo;
        public ArtisanService(ArtisanRepository repo)
        {
            _Repo = repo;
        }

        // جيب كل الحرفيين
        public async Task<List<ArtisanDto>> GetAllAsync()
        {
            var artisans = await _Repo.GetAllAsync();

            return artisans.Select(a => new ArtisanDto
            {
                Id = a.Id,
                UserId = a.UserId,
                Bio = a.Bio,
                City = a.City,
                Rating = a.Rating,
                TotalReviews = a.TotalReviews,
                IsAvailable = a.IsAvailable,
                IsVerified = a.IsVerified,
                CategoryName = a.Category.Name
            }).ToList();
        }

        // جيب حرفي بالـ ID
        public async Task<ArtisanDto?> GetByIdAsync(int id)
        {
            var artisan = await _Repo.GetByIdAsync(id);
            if (artisan == null) return null;

            return new ArtisanDto   // Model → DTO هنا
            {
                Id = artisan.Id,
                UserId = artisan.UserId,
                Bio = artisan.Bio,
                City = artisan.City,
                Rating = artisan.Rating,
                TotalReviews = artisan.TotalReviews,
                IsAvailable = artisan.IsAvailable,
                IsVerified = artisan.IsVerified,
                CategoryName = artisan.Category.Name
            };
        }

        // أضف Profile جديد
        public async Task CreateProfileAsync(CreateArtisanProfileDto dto)
        {
            var exists = await _Repo.NationalIdExistsAsync(dto.NationalId);
            if (exists)
                throw new InvalidOperationException("National ID already exists.");

            var artisan = new ArtisanProfile   //  DTO → Model هنا
            {
                UserId = dto.UserId,
                CategoryId = dto.CategoryId,
                NationalId = dto.NationalId,
                Bio = dto.Bio,
                City = dto.City
            };

            await _Repo.AddAsync(artisan);
        }

        // عدّل Profile
        public async Task<bool> UpdateProfileAsync(int id, UpdateArtisanProfileDto dto)
        {
            var artisan = await _Repo.GetByIdAsync(id);
            if (artisan == null) return false;

            if (dto.Bio != null) artisan.Bio = dto.Bio;
            if (dto.City != null) artisan.City = dto.City;
            if (dto.CategoryId != null) artisan.CategoryId = dto.CategoryId.Value;

            await _Repo.UpdateAsync(artisan);
            return true;
        }

        // تحقق من حرفي
        public async Task<bool> VerifyAsync(int id)
        {
            return await _Repo.VerifyAsync(id);
        }


        // فلترة بالتصنيف
        public async Task<List<ArtisanDto>> GetByCategoryAsync(int categoryId)
        {
            var artisans = await _Repo.getByCategory(categoryId);

            return artisans.Select(a => new ArtisanDto
            {
                Id = a.Id,
                UserId = a.UserId,
                Bio = a.Bio,
                City = a.City,
                Rating = a.Rating,
                TotalReviews = a.TotalReviews,
                IsAvailable = a.IsAvailable,
                IsVerified = a.IsVerified,
                CategoryName = a.Category?.Name
            }).ToList();
        }

        // فلترة بالمدينة
        public async Task<List<ArtisanDto>> GetByCityAsync(string city)
        {
            var artisans = await _Repo.getByCity(city);

            return artisans.Select(a => new ArtisanDto
            {
                Id = a.Id,
                UserId = a.UserId,
                Bio = a.Bio,
                City = a.City,
                Rating = a.Rating,
                TotalReviews = a.TotalReviews,
                IsAvailable = a.IsAvailable,
                IsVerified = a.IsVerified,
                CategoryName = a.Category?.Name
            }).ToList();
        }


        // تفعيل/تعطيل الإتاحة
        public async Task<bool> ToggleAvailabilityAsync(int artisanId)
        {
            return await _Repo.ToggleAvailabilityAsync(artisanId);
        }

        // إعادة حساب التقييم
        public async Task RecalculateRatingAsync(int artisanId)
        {
            await _Repo.RecalculateRatingAsync(artisanId);
        }
    }
}

using Herfa_back.DTOs.Artisan;
using Herfa_back.Interfaces.IRepository;
using Herfa_back.Interfaces.IService;
using Herfa_back.Models;
using Microsoft.AspNetCore.Hosting;

namespace Herfa_back.Services
{
    public class ArtisanService : IArtisanService
    {
        private readonly IArtisanRepository _Repo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ArtisanService(IArtisanRepository repo, IWebHostEnvironment webHostEnvironment)
        {
            _Repo = repo;
            _webHostEnvironment = webHostEnvironment;
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

            // Save the image file and get the path
            string? dbImagePath = null;
            if(dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                //1- Save the image file to the images folder in wwwroot and get the relative path
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                // Ensure the uploads folder exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // 2. توليد اسم فريد للصورة عشان لو حرفيين رفعوا صورتين بنفس الاسم ميمسحوش بعض
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + dto.ImageFile.FileName;

                //3- حفظ الصورة في المجلد
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                //4- حفظ الصورة في المجلد
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(fileStream);
                }

                dbImagePath = Path.Combine("images", uniqueFileName); // Save the relative path to the database
            }
            // 5- إنشاء نموذج الحرفي الجديد
            var artisan = new ArtisanProfile   //  DTO → Model هنا
            {
                UserId = dto.UserId,
                CategoryId = dto.CategoryId,
                NationalId = dto.NationalId,
                Bio = dto.Bio,
                City = dto.City,
                ImagePath = dbImagePath
            };

            await _Repo.AddAsync(artisan);
        }

        // غيّر الصورة الشخصية
        public async Task UpdateProfileImageAsync(int id, IFormFile imageFile)
        {
            var artisan = await _Repo.GetByIdAsync(id);
            if (artisan == null)
                throw new KeyNotFoundException("Artisan not found.");

            if (imageFile == null || imageFile.Length == 0)
                throw new InvalidOperationException("Please select an image file.");

            // حذف الصورة القديمة لو موجودة
            if (!string.IsNullOrEmpty(artisan.ImagePath))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, artisan.ImagePath);
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }

            // حفظ الصورة الجديدة
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // تحديث المسار في الداتابيز
            artisan.ImagePath = Path.Combine("images", uniqueFileName);
            await _Repo.UpdateAsync(artisan);
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

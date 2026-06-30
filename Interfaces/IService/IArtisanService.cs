using Herfa_back.DTOs.Artisan;
using Herfa_back.Models;

namespace Herfa_back.Interfaces.IService
{
    public interface IArtisanService
    {
        Task<List<ArtisanDto>> GetAllAsync();
        Task<ArtisanDto?> GetByIdAsync(int id);
        Task CreateProfileAsync(CreateArtisanProfileDto dto);
        Task<bool> UpdateProfileAsync(int id, UpdateArtisanProfileDto dto);
        Task<bool> VerifyAsync(int id);
        Task<List<ArtisanDto>> GetByCategoryAsync(int categoryId);
        Task<List<ArtisanDto>> GetByCityAsync(string city);
        Task<bool> ToggleAvailabilityAsync(int artisanId);
        Task RecalculateRatingAsync(int artisanId);
    }
}

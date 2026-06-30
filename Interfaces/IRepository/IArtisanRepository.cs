using Herfa_back.Models;

namespace Herfa_back.Interfaces.IRepository
{
    public interface IArtisanRepository
    {
        Task<List<ArtisanProfile>> getByCategory(int categoryId);
        Task<List<ArtisanProfile>> getByCity(string city);
        Task<bool> ToggleAvailabilityAsync(int artisanId);
        Task RecalculateRatingAsync(int artisanId);
        Task<List<ArtisanProfile>> GetAllAsync();
        Task<ArtisanProfile?> GetByIdAsync(int id);
        Task AddAsync(ArtisanProfile artisan);
        Task UpdateAsync(ArtisanProfile artisan);
        Task<bool> VerifyAsync(int id);
        Task<bool> NationalIdExistsAsync(string nationalId);
    }
}

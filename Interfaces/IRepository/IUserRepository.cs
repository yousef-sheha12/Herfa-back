using Herfa_back.Models;

namespace Herfa_back.Interfaces.IRepository
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task SaveChangesAsync();
        Task AddPasswordResetTokenAsync(PasswordResetToken resetToken);
        Task<PasswordResetToken?> GetPasswordResetTokenAsync(string token, string email);
        Task AddBlacklistedTokenAsync(BlacklistedToken blacklistedToken);
        Task<bool> IsTokenBlacklistedAsync(string jti);
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(string token);
        Task RevokeUserRefreshTokensAsync(int userId);
    }
}

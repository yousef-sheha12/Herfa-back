using Herfa_back.DTOs.Auth;

namespace Herfa_back.Interfaces.IService
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<string> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task ResetPasswordAsync(ResetPasswordDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto);
        Task LogoutAsync(string token);
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using BCrypt.Net;
using Herfa_back.DTOs.Auth;
using Herfa_back.Helpers;
using Herfa_back.Interfaces.IRepository;
using Herfa_back.Interfaces.IService;
using Herfa_back.Models;

namespace Herfa_back.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;
        public AuthService(
            IUserRepository userRepository,
            JwtHelper jwtHelper
        )
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }
        private async Task<string> GenerateRefreshTokenAsync(int userId)
        {
            var refreshTokenValue = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };

            await _userRepository.AddRefreshTokenAsync(refreshToken);
            return refreshTokenValue;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userRepository
                .GetByEmailAsync(dto.Email);

            if (existingUser is not null)
                throw new Exception("Email already exists");

            var user = new User
            {
                Username = dto.UserName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.role
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var accessToken = _jwtHelper.GenerateToken(user);
            var refreshToken = await GenerateRefreshTokenAsync(user.Id);
            await _userRepository.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                User = new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role.ToString()
                }
            };
        }

            public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository
                .GetByEmailAsync(dto.Email);

            if (user is null)
                throw new Exception("Invalid email or password");

            var isPasswordValid = BCrypt.Net.BCrypt
                .Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
                throw new Exception("Invalid email or password");

            var accessToken = _jwtHelper.GenerateToken(user);
            var refreshToken = await GenerateRefreshTokenAsync(user.Id);
            await _userRepository.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                User = new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role.ToString()
                }
            };
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user is not null)
            {
                var resetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
                var resetTokenEntity = new PasswordResetToken
                {
                    Email = dto.Email,
                    Token = resetToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1)
                };

                await _userRepository.AddPasswordResetTokenAsync(resetTokenEntity);
                await _userRepository.SaveChangesAsync();
            }
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var resetTokenEntity = await _userRepository
                .GetPasswordResetTokenAsync(dto.Token, dto.Email);

            if (resetTokenEntity is null || resetTokenEntity.IsUsed || resetTokenEntity.ExpiresAt < DateTime.UtcNow)
                throw new Exception("Invalid or expired reset token");

            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user is null)
                throw new Exception("Invalid or expired reset token");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            resetTokenEntity.IsUsed = true;

            await _userRepository.RevokeUserRefreshTokensAsync(user.Id);

            await _userRepository.SaveChangesAsync();
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var storedToken = await _userRepository.GetRefreshTokenAsync(dto.RefreshToken);

            if (storedToken is null)
                throw new Exception("Invalid refresh token");

            if (storedToken.IsRevoked)
                throw new Exception("Refresh token has been revoked");

            if (storedToken.ExpiresAt < DateTime.UtcNow)
                throw new Exception("Refresh token has expired");

            var user = await _userRepository.GetByIdAsync(storedToken.UserId);

            if (user is null)
                throw new Exception("User not found");

            await _userRepository.RevokeRefreshTokenAsync(dto.RefreshToken);

            var newAccessToken = _jwtHelper.GenerateToken(user);
            var newRefreshToken = await GenerateRefreshTokenAsync(user.Id);
            await _userRepository.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                User = new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role.ToString()
                }
            };
        }

        public async Task LogoutAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("Token is required");

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var jti = jwtToken.Claims
                .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (string.IsNullOrWhiteSpace(jti))
                throw new Exception("Invalid token");

            var blacklisted = new BlacklistedToken
            {
                Jti = jti,
                ExpiresAt = jwtToken.ValidTo
            };

            await _userRepository.AddBlacklistedTokenAsync(blacklisted);

            var userIdClaim = jwtToken.Claims
                .FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out var userId))
            {
                await _userRepository.RevokeUserRefreshTokensAsync(userId);
            }

            await _userRepository.SaveChangesAsync();
        }
    }

}

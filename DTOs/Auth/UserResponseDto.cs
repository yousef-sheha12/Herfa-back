using Herfa_back.Models;
namespace Herfa_back.DTOs.Auth
{
    public class UserResponseDto : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; 
        public string? AvatarUrl { get; set; }
    }
}

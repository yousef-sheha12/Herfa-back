using Herfa_back.Models.Enums;

namespace Herfa_back.Models
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; }= string.Empty;
        public string? AvatarUrl { get; set; }
        public UserRole Role { get; set; }

        public ArtisanProfile? ArtisanProfile { get; set; }
        public ICollection<ServiceRequest> ServiceRequests { get; set; }
        public ICollection<Notification> Notifications { get; set; }


    }
}

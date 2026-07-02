using Herfa_back.Models.Enums;

namespace Herfa_back.Models
{
    public class Notification : BaseEntity
    {
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; } = false;
        public int? ReferenceId { get; set; }
        public decimal? Price { get; set; }
        public string? Comment { get; set; }

        // Navigation
        public User User { get; set; } = null!;
    }
}

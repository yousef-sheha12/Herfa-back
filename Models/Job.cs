namespace Herfa_back.Models
{
    public class Job : BaseEntity
    {
        public int ServiceRequestId { get; set; }
        public int ArtisanId { get; set; }
        public int ClientId { get; set; }
        public JobStatus Status { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        public ServiceRequest ServiceRequest { get; set; } = null!;
        public ArtisanProfile Artisan { get; set; } = null!;
        public User Client { get; set; } = null!;
        public Review? Review { get; set; }
    }
}
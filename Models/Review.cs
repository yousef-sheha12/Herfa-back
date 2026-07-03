namespace Herfa_back.Models
{
    public class Review : BaseEntity
    {
        public int JobId { get; set; }
        public int ClientId { get; set; }
        public int ArtisanId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }

        // Navigation properties
        public Job Job { get; set; } = null!;
        public User Client { get; set; } = null!;
        public ArtisanProfile Artisan { get; set; } = null!;
    }
}

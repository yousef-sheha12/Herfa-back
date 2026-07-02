namespace Herfa_back.DTOs.Review
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int ArtisanId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

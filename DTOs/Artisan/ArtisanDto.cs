namespace Herfa_back.DTOs.Artisan
{
    public class ArtisanDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Bio { get; set; }
        public string? City { get; set; }
        public float Rating { get; set; }
        public int TotalReviews { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsVerified { get; set; }
        public string CategoryName { get; set; }
    }
}

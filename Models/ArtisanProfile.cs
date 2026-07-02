namespace Herfa_back.Models
{
    public class ArtisanProfile : BaseEntity
    {
        public int UserId { get; set; }          // FK → User
        public int CategoryId { get; set; }      // FK → Category
        public string NationalId { get; set; }   // 15 digits, unique
        public string? Bio { get; set; }
        public string? City { get; set; }
        public float Rating { get; set; } = 0;   // متوسط التقييم
        public int TotalReviews { get; set; } = 0;
        public bool IsAvailable { get; set; } = true;
        public bool IsVerified { get; set; } = false;

        // Navigation
        public User User { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public ICollection<ServiceOffer> Offers { get; set; } = new List<ServiceOffer>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}

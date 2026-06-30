namespace Herfa_back.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public string? Description { get; set; }
        // Navigation
        public List<ArtisanProfile> Artisans { get; set; } = new List<ArtisanProfile>();
        public List<ServiceRequest> Requests { get; set; } = new List<ServiceRequest>();
    }
}

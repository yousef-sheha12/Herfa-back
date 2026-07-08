using Herfa_back.DTOs.Offer;

namespace Herfa_back.DTOs.Artisan
{
    public class CreateArtisanProfileDto
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string NationalId { get; set; }
        public string? Bio { get; set; }
        public string? City { get; set; }
        public InitialServiceOfferDto InitialService { get; set; } = null!;
        public IFormFile ImageFile { get; set; }
    }
}

namespace Herfa_back.DTOs.Artisan
{
    public class CreateArtisanProfileDto
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string NationalId { get; set; }
        public string? Bio { get; set; }
        public string? City { get; set; }
    }
}

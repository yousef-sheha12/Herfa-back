namespace Herfa_back.DTOs.Offer
{
    public class OfferDto
    {// to show the offer details 
        public int Id { get; set; }
        public int ArtisanId { get; set; }
        public string ArtisanName { get; set; }
        public decimal Price { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}

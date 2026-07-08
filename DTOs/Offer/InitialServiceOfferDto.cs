namespace Herfa_back.DTOs.Offer
{
    public class InitialServiceOfferDto
    {
        public string Title { get; set; } = null!; // اسم الخدمة
        public string Description { get; set; } = null!; // وصف الخدمة
        public decimal Price { get; set; } // السعر
    }
}

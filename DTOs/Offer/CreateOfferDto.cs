using System.ComponentModel.DataAnnotations;

namespace Herfa_back.DTOs.Offer
{
    public class CreateOfferDto
    {
        [Range(1, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public string Message { get; set; }
    
    }
}

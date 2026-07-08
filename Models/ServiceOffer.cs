using Herfa_back.Models.Enums;

namespace Herfa_back.Models
{
    public class ServiceOffer : BaseEntity
    {
        public int ServiceRequestId { get; set; }
        public int ArtisanId { get; set; }
        public string Message { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public ServiceOfferStatus Status { get; set; }

        //Navigation
         //internal relationship
        public ServiceRequest ServiceRequest { get; set; } //many->1

        //external relationship
        public ArtisanProfile Artisan { get; set; } //many->1
    }
}

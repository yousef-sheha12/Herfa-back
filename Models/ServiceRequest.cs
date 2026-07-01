using Herfa_back.Models.Enums;

namespace Herfa_back.Models

{
    public class ServiceRequest : BaseEntity 
    {
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string? ImageUrl { get; set; }
        public ServiceRequestStatus Status { get; set; }
        public int ClientId { get; set; }

        // Navigation
        //internal relationships
        public ICollection<ServiceOffer> Offers { get; set; } //1->many

          //external relationships
        public User Client { get; set; } //many->1
        public Category Category { get; set; } //many->1
        public Job? Job { get; set; } //1->1
    }
}

using Herfa_back.DTOs.Offer;

namespace Herfa_back.DTOs.Request 
{
    public class ServiceRequestDetailsDto
    { // get all details of selected request
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string? ImageUrl { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CategoryName { get; set; }
        public ICollection<OfferDto> Offers { get; set; }

    }
}

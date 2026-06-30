namespace Herfa_back.DTOs.Request
{
    public class CreateServiceRequestDto
    { //for new requestest client make 
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string? ImageUrl { get; set; }

    }
}

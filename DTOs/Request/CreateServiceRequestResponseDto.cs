namespace Herfa_back.DTOs.Request
{
    public class CreateServiceRequestResponseDto
    {
        public string ClientName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}

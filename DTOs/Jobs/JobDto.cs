namespace Herfa_back.DTOs.Jobs
{
    public class JobDto
    {
        public int Id { get; set; }
        public int ServiceRequestId { get; set; }
        public int ArtisanId { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string ArtisanName { get; set; }
        public string ServiceRequestTitle { get; set; }
        public string Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
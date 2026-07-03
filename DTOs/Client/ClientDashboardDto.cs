namespace Herfa_back.DTOs.Client
{
    public class ClientDashboardDto
    {
        public int TotalRequests { get; set; }
        public int ActiveRequests { get; set; }
        public int CompletedJobs { get; set; }
        public decimal TotalSpent { get; set; }
    }
}

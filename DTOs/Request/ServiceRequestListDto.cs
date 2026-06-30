namespace Herfa_back.DTOs.Request
{
    public class ServiceRequestListDto
    {
        // get all requestes 
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt {  get; set; }
        public string CategoryName { get; set; }
    }
}

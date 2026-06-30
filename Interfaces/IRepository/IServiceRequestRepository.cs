using Herfa_back.Models;
namespace Herfa_back.Interfaces.IRepository
{
    public interface IServiceRequestRepository
    {
        Task<ServiceRequest> AddAsync(ServiceRequest serviceRequest);// POST /requests -> create a new service request
        Task<ServiceRequest?> GetByIdAsync(int id);// GET /requests/{id} -> get request details
        Task<IEnumerable<ServiceRequest>> GetAllAsync();//GET /requests -> get all available requests
        Task UpdateAsync(ServiceRequest serviceRequest);//update the request data or request status
        Task SaveChangesAsync();//save changes to database  
    }
}

   


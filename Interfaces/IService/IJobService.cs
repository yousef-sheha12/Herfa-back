using Herfa_back.DTOs.Jobs;

namespace Herfa_back.Interfaces.IService
{
    public interface IJobService
    {
        Task<List<JobDto>> GetAllJobsAsync(int userId, string role);
        Task<JobDto?> GetJobByIdAsync(int jobId, int userId, string role);
        Task<JobDto> CreateJobAsync(int serviceRequestId, int artisanId, int clientId);
        Task<bool> CompleteJobAsync(int jobId, int userId);
        Task<bool> CancelJobAsync(int jobId, int userId);
    }
}
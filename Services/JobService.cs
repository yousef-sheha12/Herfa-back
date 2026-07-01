using Herfa_back.DTOs.Jobs;
using Herfa_back.Interfaces.IService;
using Herfa_back.Models;
using Herfa_back.Repositories;

namespace Herfa_back.Services
{
    public class JobService : IJobService
    {
        private readonly JobRepository _repo;

        public JobService(JobRepository repo)
        {
            _repo = repo;
        }

        // Get all jobs filtered by role
        public async Task<List<JobDto>> GetAllJobsAsync(int userId, string role)
        {
            List<Job> jobs;

            if (role == "Admin")
                jobs = await _repo.GetAllAsync();
            else if (role == "Artisan")
                jobs = await _repo.GetByArtisanAsync(userId);
            else
                jobs = await _repo.GetByClientAsync(userId);

            return jobs.Select(j => MapToDto(j)).ToList();
        }

        // Get single job by id with ownership check
        public async Task<JobDto?> GetJobByIdAsync(int jobId, int userId, string role)
        {
            var job = await _repo.GetByIdAsync(jobId);
            if (job == null) return null;

            // check if the requester owns this job
            if (role != "Admin" && job.ClientId != userId && job.ArtisanId != userId)
                return null;

            return MapToDto(job);
        }

        // Called by Person 3 inside their accept-offer transaction
        public async Task<JobDto> CreateJobAsync(int serviceRequestId, int artisanId, int clientId)
        {
            var job = new Job
            {
                ServiceRequestId = serviceRequestId,
                ArtisanId = artisanId,
                ClientId = clientId,
                Status = JobStatus.InProgress,
                StartedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(job);
            return MapToDto(job);
        }

        // Client marks job as complete
        public async Task<bool> CompleteJobAsync(int jobId, int userId)
        {
            var job = await _repo.GetByIdAsync(jobId);
            if (job == null) return false;

            // only the client who owns this job can complete it
            if (job.ClientId != userId) return false;

            // only InProgress jobs can be completed
            if (job.Status != JobStatus.InProgress) return false;

            job.Status = JobStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;

            // TODO: update ServiceRequest status to Completed (Person 3's service)
            // TODO: trigger notification to artisan (Person 5's service)

            await _repo.UpdateAsync(job);
            return true;
        }

        // Client cancels the job
        public async Task<bool> CancelJobAsync(int jobId, int userId)
        {
            var job = await _repo.GetByIdAsync(jobId);
            if (job == null) return false;

            // only the client who owns this job can cancel it
            if (job.ClientId != userId) return false;

            // only InProgress jobs can be cancelled
            if (job.Status != JobStatus.InProgress) return false;

            job.Status = JobStatus.Cancelled;

            // TODO: update ServiceRequest status (Person 3's service)
            // TODO: trigger notification to artisan (Person 5's service)

            await _repo.UpdateAsync(job);
            return true;
        }

        // Private helper - Model → DTO mapping (used everywhere)
        private JobDto MapToDto(Job job)
        {
            return new JobDto
            {
                Id = job.Id,
                ServiceRequestId = job.ServiceRequestId,
                ArtisanId = job.ArtisanId,
                ClientId = job.ClientId,
                ClientName = string.Empty,   // will fill when User nav property is uncommented
                ArtisanName = string.Empty,  // will fill when ArtisanProfile nav property is uncommented
                ServiceRequestTitle = string.Empty, // will fill when ServiceRequest nav property is uncommented
                Status = job.Status.ToString(),
                StartedAt = job.StartedAt,
                CompletedAt = job.CompletedAt,
                CreatedAt = job.CreatedAt
            };
        }
    }
}
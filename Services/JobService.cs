using Herfa_back.DTOs.Jobs;
using Herfa_back.Interfaces.IService;
using Herfa_back.Models;
using Herfa_back.Models.Enums;
using Herfa_back.Repositories;

namespace Herfa_back.Services
{
    public class JobService : IJobService
    {
        private readonly JobRepository _repo;
        private readonly INotificationService _notificationService;// after Person 5

        public JobService(JobRepository repo, INotificationService notificationService)// after Person 5
        {
            _repo = repo;
            _notificationService = notificationService;
        }

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

        public async Task<JobDto?> GetJobByIdAsync(int jobId, int userId, string role)
        {
            var job = await _repo.GetByIdAsync(jobId);
            if (job == null) return null;

            if (role != "Admin" && job.ClientId != userId && job.ArtisanId != userId)
                return null;

            return MapToDto(job);
        }

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

        public async Task<bool> CompleteJobAsync(int jobId, int userId)
        {
            var job = await _repo.GetByIdAsync(jobId);
            if (job == null) return false;
            if (job.ClientId != userId) return false;
            if (job.Status != JobStatus.InProgress) return false;

            // Update job status
            job.Status = JobStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(job);

            // Update ServiceRequest status to Completed
            await _repo.UpdateServiceRequestStatusAsync(job.ServiceRequestId, ServiceRequestStatus.Completed);

            // Notify the artisan
            await _notificationService.SendAsync(
                userId: job.ArtisanId,
                title: "Job Completed",
                message: $"The client has marked job #{job.Id} as completed.",
                type: "JobCompleted"
            );

            return true;
        }

        public async Task<bool> CancelJobAsync(int jobId, int userId)
        {
            var job = await _repo.GetByIdAsync(jobId);
            if (job == null) return false;
            if (job.ClientId != userId) return false;
            if (job.Status != JobStatus.InProgress) return false;

            // Update job status
            job.Status = JobStatus.Cancelled;
            await _repo.UpdateAsync(job);

            // Update ServiceRequest status to Cancelled
            await _repo.UpdateServiceRequestStatusAsync(job.ServiceRequestId, ServiceRequestStatus.Cancelled);

            // Notify the artisan
            await _notificationService.SendAsync(
                userId: job.ArtisanId,
                title: "Job Cancelled",
                message: $"The client has cancelled job #{job.Id}.",
                type: "JobCancelled"
            );

            return true;
        }

        private JobDto MapToDto(Job job)
        {
            return new JobDto
            {
                Id = job.Id,
                ServiceRequestId = job.ServiceRequestId,
                ArtisanId = job.ArtisanId,
                ClientId = job.ClientId,
                ClientName = job.Client?.Username ?? string.Empty,
                ArtisanName = job.Artisan?.User?.Username ?? string.Empty,
                ServiceRequestTitle = job.ServiceRequest?.Title ?? string.Empty,
                Status = job.Status.ToString(),
                StartedAt = job.StartedAt,
                CompletedAt = job.CompletedAt,
                CreatedAt = job.CreatedAt
            };
        }
    }
}
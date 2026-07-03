using Herfa_back.Data;
using Herfa_back.DTOs.Notification;
using Herfa_back.DTOs.Review;
using Herfa_back.Interfaces.IRepository;
using Herfa_back.Interfaces.IService;
using Herfa_back.Models;
using Herfa_back.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Herfa_back.Services
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;
        private readonly IReviewRepository _reviewRepository;
        private readonly IArtisanRepository _artisanRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(
            AppDbContext context,
            IReviewRepository reviewRepository,
            IArtisanRepository artisanRepository,
            INotificationService notificationService,
            ILogger<ReviewService> logger)
        {
            _context = context;
            _reviewRepository = reviewRepository;
            _artisanRepository = artisanRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<ReviewDto> SubmitReviewAsync(int jobId, int clientUserId, CreateReviewDto dto)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null || job.Status != JobStatus.Completed || job.ClientId != clientUserId)
            {
                throw new InvalidOperationException("Job not found, not completed, or you are not the client.");
            }

            var existingReview = await _reviewRepository.GetByJobIdAsync(jobId);
            if (existingReview != null)
            {
                throw new InvalidOperationException("A review already exists for this job.");
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            ReviewDto reviewDtoResult = null!;

            await strategy.ExecuteAsync(async () =>
            {
                using var tx = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. Create Review
                    var review = new Review
                    {
                        JobId = jobId,
                        ClientId = clientUserId,
                        ArtisanId = job.ArtisanId, // Ensure this points to ArtisanProfile.Id
                        Rating = dto.Rating,
                        Comment = dto.Comment,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _reviewRepository.AddAsync(review);

                    // 2. Update ArtisanProfile incrementally
                    var artisan = await _artisanRepository.GetByIdAsync(job.ArtisanId);
                    if (artisan != null)
                    {
                        var newRating = (artisan.Rating * artisan.TotalReviews + dto.Rating) / (artisan.TotalReviews + 1);
                        artisan.TotalReviews += 1;
                        artisan.Rating = newRating;
                        // Note: UpdateAsync inside ArtisanRepository might call SaveChanges,
                        // we want to avoid that to let the explicit transaction handle it,
                        // but EF tracks the artisan entity so modifying it is enough,
                        // and we will call _context.SaveChangesAsync() next.
                    }

                    // 3. Create Notification and Push
                    await _notificationService.SendAsync(
                        userId: artisan?.UserId ?? 0, // Using UserId from ArtisanProfile
                        title: "New Review Received",
                        message: $"You received a {dto.Rating}-star review for job #{jobId}.",
                        type: NotificationType.NewReview,
                        referenceId: review.Id
                    );

                    await _context.SaveChangesAsync();
                    await tx.CommitAsync();

                    reviewDtoResult = new ReviewDto
                    {
                        Id = review.Id,
                        JobId = review.JobId,
                        ClientId = review.ClientId,
                        ClientName = "", // Will be populated in controller or via Include if needed
                        ArtisanId = review.ArtisanId,
                        Rating = review.Rating,
                        Comment = review.Comment,
                        CreatedAt = review.CreatedAt
                    };
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    _logger.LogError(ex, "Failed to submit review for job {JobId}", jobId);
                    throw;
                }
            });

            return reviewDtoResult;
        }

        public async Task<ReviewDto?> GetReviewByJobIdAsync(int jobId)
        {
            var review = await _reviewRepository.GetByJobIdAsync(jobId);
            if (review == null) return null;

            return new ReviewDto
            {
                Id = review.Id,
                JobId = review.JobId,
                ClientId = review.ClientId,
                ClientName = review.Client?.Username ?? string.Empty,
                ArtisanId = review.ArtisanId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };
        }

        public async Task<List<ReviewDto>> GetReviewsByArtisanIdAsync(int artisanId)
        {
            var reviews = await _reviewRepository.GetByArtisanIdAsync(artisanId);

            return reviews.Select(review => new ReviewDto
            {
                Id = review.Id,
                JobId = review.JobId,
                ClientId = review.ClientId,
                ClientName = review.Client?.Username ?? string.Empty,
                ArtisanId = review.ArtisanId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            }).ToList();
        }
    }
}

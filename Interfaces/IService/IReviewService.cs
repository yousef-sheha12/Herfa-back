using Herfa_back.DTOs.Review;

namespace Herfa_back.Interfaces.IService
{
    public interface IReviewService
    {
        Task<ReviewDto> SubmitReviewAsync(int jobId, int clientUserId, CreateReviewDto dto);
        Task<ReviewDto?> GetReviewByJobIdAsync(int jobId);
        Task<List<ReviewDto>> GetReviewsByArtisanIdAsync(int artisanId);
    }
}

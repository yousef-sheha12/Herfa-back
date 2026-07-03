using Herfa_back.DTOs.Review;
using Herfa_back.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Herfa_back.Controllers
{
    [ApiController]
    [Route("api/jobs/{jobId}/review")]
    [Authorize] // Client role should probably be enforced here, but we check role in claims or just the client id match in service
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReview(int jobId, [FromBody] CreateReviewDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "Client")
            {
                return Forbid("Only clients can submit reviews.");
            }

            try
            {
                var review = await _reviewService.SubmitReviewAsync(jobId, userId, dto);
                return Created($"/api/jobs/{jobId}/review", review);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("already exists"))
                    return Conflict(ex.Message);
                    
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while submitting the review.");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewByJobId(int jobId)
        {
            var review = await _reviewService.GetReviewByJobIdAsync(jobId);
            if (review == null)
            {
                return NotFound("Review not found for this job.");
            }
            return Ok(review);
        }
    }
}

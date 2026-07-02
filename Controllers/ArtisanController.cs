using Herfa_back.DTOs.Artisan;
using Herfa_back.Interfaces.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Herfa_back.Controllers
{
    [ApiController]
    [Route("api/artisans")]
    public class ArtisanController : ControllerBase
    {
        private readonly IArtisanService _service;

        public ArtisanController(IArtisanService service)
        {
            _service = service;
        }

        // GET /artisans
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var artisans = await _service.GetAllAsync();
            return Ok(artisans);
        }

        // GET /artisans/:id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var artisan = await _service.GetByIdAsync(id);
            if (artisan == null)
                return NotFound("Artisan not found.");

            return Ok(artisan);
        }

        // POST /artisans/profile
        [HttpPost("profile")]
        public async Task<IActionResult> CreateProfile(CreateArtisanProfileDto dto)
        {
            try
            {
                await _service.CreateProfileAsync(dto);
                return Ok("Profile created successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // PUT /artisans/profile/:id
        [HttpPut("profile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, UpdateArtisanProfileDto dto)
        {
            return await _service.UpdateProfileAsync(id, dto)
                ? Ok("Profile updated successfully.")
                : NotFound("Artisan not found.");
        }

        // PATCH /artisans/:id/verify
        [HttpPatch("{id}/verify")]
        public async Task<IActionResult> Verify(int id)
        {
            return await _service.VerifyAsync(id)
                ? Ok("Artisan verified successfully.")
                : NotFound("Artisan not found.");
        }

        // GET /artisans/:id/reviews
        [HttpGet("{id}/reviews")]
        public async Task<IActionResult> GetReviews(int id, [FromServices] IReviewService reviewService)
        {
            var reviews = await reviewService.GetReviewsByArtisanIdAsync(id);
            return Ok(reviews);
        }
    }
}

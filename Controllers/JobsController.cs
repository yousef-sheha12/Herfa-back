using Herfa_back.DTOs.Jobs;
using Herfa_back.Services;
using Microsoft.AspNetCore.Mvc;

namespace Herfa_back.Controllers
{
    [ApiController]
    [Route("api/jobs")]
    public class JobsController : ControllerBase
    {
        private readonly JobService _service;

        public JobsController(JobService service)
        {
            _service = service;
        }

        // GET /jobs
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // TODO: replace hardcoded values with actual JWT claims
            // when Person 1's auth middleware is ready
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var role = User.FindFirst("role")?.Value ?? "Client";

            var jobs = await _service.GetAllJobsAsync(userId, role);
            return Ok(jobs);
        }

        // GET /jobs/:id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // TODO: replace hardcoded values with actual JWT claims
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var role = User.FindFirst("role")?.Value ?? "Client";

            var job = await _service.GetJobByIdAsync(id, userId, role);
            if (job == null)
                return NotFound("Job not found.");

            return Ok(job);
        }

        // PATCH /jobs/:id/complete
        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            // TODO: replace hardcoded values with actual JWT claims
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

            var result = await _service.CompleteJobAsync(id, userId);
            if (!result)
                return BadRequest("Job not found, already completed, or you are not the client.");

            return Ok("Job marked as completed successfully.");
        }

        // PATCH /jobs/:id/cancel
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            // TODO: replace hardcoded values with actual JWT claims
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

            var result = await _service.CancelJobAsync(id, userId);
            if (!result)
                return BadRequest("Job not found, already cancelled, or you are not the client.");

            return Ok("Job cancelled successfully.");
        }
    }
}
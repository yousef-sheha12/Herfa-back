using Herfa_back.DTOs.Jobs;
using Herfa_back.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Herfa_back.Controllers
{
    [ApiController]
    [Route("api/jobs")]
    [Authorize]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _service;

        public JobsController(IJobService service)
        {
            _service = service;
        }

        // GET /jobs
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();
            
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "Client";

            var jobs = await _service.GetAllJobsAsync(userId, role);
            return Ok(jobs);
        }

        // GET /jobs/:id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "Client";

            var job = await _service.GetJobByIdAsync(id, userId, role);
            if (job == null)
                return NotFound("Job not found.");

            return Ok(job);
        }

        // PATCH /jobs/:id/complete
        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var result = await _service.CompleteJobAsync(id, userId);
            if (!result)
                return BadRequest("Job not found, already completed, or you are not the client.");

            return Ok("Job marked as completed successfully.");
        }

        // PATCH /jobs/:id/cancel
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var result = await _service.CancelJobAsync(id, userId);
            if (!result)
                return BadRequest("Job not found, already cancelled, or you are not the client.");

            return Ok("Job cancelled successfully.");
        }
    }
}
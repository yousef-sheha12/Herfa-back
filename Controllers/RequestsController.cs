using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Herfa_back.DTOs.Request;
using Herfa_back.Services;
using Herfa_back.Interfaces.IService;

namespace Herfa_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly IServiceRequestService _requestService;

        public RequestsController(IServiceRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateServiceRequestDto dto)
        {
            int clientId = 1; // Temporary until JWT authentication is implemented

            var result = await _requestService.CreateRequestAsync(dto, clientId);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _requestService.GetAllRequestsAsync();

            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var request = await _requestService.GetRequestByIdAsync(id);

            if (request == null)
                return NotFound();

            return Ok(request);
        }

        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            await _requestService.CancelRequestAsync(id);

            return NoContent();
        }
    }
}

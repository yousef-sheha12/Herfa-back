using Herfa_back.DTOs.Client;
using Herfa_back.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Herfa_back.Controllers
{
    [ApiController]
    [Route("api/clients")]
    [Authorize(Roles = "Client")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet("me/dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClientDashboardDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetDashboard()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            var dashboard = await _clientService.GetDashboardStatsAsync(userId);
            return Ok(dashboard);
        }

        [HttpGet("me/profile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClientProfileDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            var profile = await _clientService.GetProfileAsync(userId);
            if (profile == null) return NotFound("Client profile not found.");

            return Ok(profile);
        }

        [HttpPut("me/profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateClientProfileDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            var success = await _clientService.UpdateProfileAsync(userId, dto);
            if (!success) return NotFound("Client profile not found.");

            return Ok(new { message = "Profile updated successfully." });
        }
    }
}

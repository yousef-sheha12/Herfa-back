using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Herfa_back.DTOs.Offer;
using Herfa_back.Services;
using Herfa_back.Interfaces.IService;

namespace Herfa_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffersController : ControllerBase
    {
        private readonly IServiceOfferService _offerService;

        public OffersController(IServiceOfferService offerService)
        {
            _offerService = offerService;
        }

        [HttpPost("request/{requestId}")]
        [Authorize(Roles = "Artisan")]
        public async Task<IActionResult> CreateOffer(int requestId, CreateOfferDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            var result = await _offerService.CreateOfferAsync(dto, requestId, userId);

            return Ok(result);
        }

        [HttpGet("request/{requestId}")]
        [Authorize]
        public async Task<IActionResult> GetOffers(int requestId)
        {
            var offers = await _offerService.GetOffersByRequestAsync(requestId);

            return Ok(offers);
        }

        [HttpPatch("{offerId}/accept")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> AcceptOffer(int offerId)
        {
            var jobId = await _offerService.AcceptOfferAsync(offerId);
            return Ok(new { message = "Offer accepted successfully.", jobId = jobId });
        }

        [HttpPatch("{offerId}/reject")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> RejectOffer(int offerId)
        {
            await _offerService.RejectOfferAsync(offerId);

            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> CreateOffer(int requestId, CreateOfferDto dto)
        {
            int artisanId = 1; // Temporary until JWT authentication is implemented

            var result = await _offerService.CreateOfferAsync(dto, requestId, artisanId);

            return Ok(result);
        }

        [HttpGet("request/{requestId}")]
        public async Task<IActionResult> GetOffers(int requestId)
        {
            var offers = await _offerService.GetOffersByRequestAsync(requestId);

            return Ok(offers);
        }

        [HttpPatch("{offerId}/accept")]
        public async Task<IActionResult> AcceptOffer(int offerId)
        {
            await _offerService.AcceptOfferAsync(offerId);

            return NoContent();
        }

        [HttpPatch("{offerId}/reject")]
        public async Task<IActionResult> RejectOffer(int offerId)
        {
            await _offerService.RejectOfferAsync(offerId);

            return NoContent();
        }
    }
}

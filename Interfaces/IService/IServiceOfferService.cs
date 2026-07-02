using Herfa_back.DTOs.Offer;
using Herfa_back.Models;

namespace Herfa_back.Interfaces.IService
{
    public interface IServiceOfferService
    {
        // Create a new offer
        Task<OfferDto> CreateOfferAsync(CreateOfferDto dto, int requestId, int artisanUserId);

        // Get all offers for a specific request
        Task<IEnumerable<OfferDto>> GetOffersByRequestAsync(int requestId);

        // Accept an offer
        Task<int> AcceptOfferAsync(int offerId);

        // Reject an offer
        Task RejectOfferAsync(int offerId);
    }
}

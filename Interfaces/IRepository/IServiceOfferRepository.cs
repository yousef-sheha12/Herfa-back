using Herfa_back.Models;
namespace Herfa_back.Interfaces.IRepository
{
    public interface IServiceOfferRepository
    {
        Task<ServiceOffer> AddAsync(ServiceOffer serviceOffer);// POST /requests/{id}/offers -> create a new offer
        Task<ServiceOffer> GetByIdAsync(int id);// get offer by id
        Task<IEnumerable<ServiceOffer>> GetByRequestIdAsync(int requestId);// GET /requests/{id}/offers -> get all offers for a request
        Task<ServiceOffer?> GetByRequestAndArtisanAsync(int requestId , int artisanId);// check if artisan already submitted an offer for this request
        Task UpdateAsync(ServiceOffer serviceOffer);// update offer status (Accepted / Rejected)
        Task SaveChangesAsync();// save changes to database
    }
}

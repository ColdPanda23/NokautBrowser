using OfferBrowser.API.API.Models;

namespace OfferBrowser.API.API.Interface;

public interface IOfferService
{
    Task<List<ScrapedOffer>> GetAllOffers();
    Task<List<ScrapedOffer>> GetAllOffersBySearchPhrase(string searchPhrase);
}
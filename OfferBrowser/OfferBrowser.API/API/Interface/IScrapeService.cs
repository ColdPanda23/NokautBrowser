using OfferBrowser.API.API.Models;

namespace OfferBrowser.API.API.Interface;

public interface IScrapeService
{
     Task<List<ScrapedOffer>> ScrapeOffer(string searchPhrase);
}
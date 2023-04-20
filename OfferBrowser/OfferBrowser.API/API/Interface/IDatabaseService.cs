using MongoDB.Driver;
using OfferBrowser.API.API.Models;

namespace OfferBrowser.API.API.Interface;

public interface IDatabaseService
{
    IMongoDatabase ConnectToDatabase();
    Task InsertScrapedOffersAsync(List<ScrapedOffer> offers);
}
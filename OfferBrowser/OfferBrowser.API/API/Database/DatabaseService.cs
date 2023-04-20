using MongoDB.Driver;
using OfferBrowser.API.API.Interface;
using OfferBrowser.API.API.Models;

namespace OfferBrowser.API.API.Database;

public class DatabaseService : IDatabaseService
{
    private  IMongoDatabase _database;

    #region Connect to database
    
    public IMongoDatabase ConnectToDatabase()
    {
        var client = new MongoClient("mongodb://localhost:27017");
        return _database = client.GetDatabase("NokautBrowser");
    }
    
    #endregion

    #region Save data to database
    
    public async Task InsertScrapedOffersAsync(List<ScrapedOffer> offers)
    {
        var collection = _database.GetCollection<ScrapedOffer>("Offers");
        await collection.InsertManyAsync(offers);
    }
    
    #endregion
}

// TODO Write test for conection to db
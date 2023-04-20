using System.Globalization;
using HtmlAgilityPack;
using MongoDB.Bson;
using MongoDB.Driver;
using OfferBrowser.API.API.Interface;
using OfferBrowser.API.API.Models;

namespace OfferBrowser.API.API.Service;

public class OfferService : IOfferService
{
    private readonly IDatabaseService _databaseService;

    public OfferService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<List<ScrapedOffer>> GetAllOffers()
    {
       await UpdateOffers();
       await UpdatePriceHistory();
        var _database = _databaseService.ConnectToDatabase();
        var collection = _database.GetCollection<ScrapedOffer>("Offers");
        var cursor = await collection.FindAsync<ScrapedOffer>(new BsonDocument());
        var offers = await cursor.ToListAsync();
        

        return offers;
    }

    public async Task UpdateOffers()
    {
        var _database = _databaseService.ConnectToDatabase();
        var offerCollection = _database.GetCollection<ScrapedOffer>("Offers");
        if (!(await _database.ListCollectionNames().ToListAsync()).Contains("OfferUpdates"))
            await _database.CreateCollectionAsync("OfferUpdates");
        var updateCollection = _database.GetCollection<OfferUpdate>("OfferUpdates");
        

        var cursor = await offerCollection.FindAsync<ScrapedOffer>(new BsonDocument());
        var offers = await cursor.ToListAsync();

        foreach (var offer in offers)
        {
            var nokautUrl = $"{offer.Link}";
            var nokautHtml = await GetWebPageHtml(nokautUrl);
            var nokautPrice = ParseNokautPrice(nokautHtml);

            if (nokautPrice != offer.Price)
            {
                var update = new OfferUpdate
                {
                    UpdatedOfferId = ObjectId.GenerateNewId().ToString(),
                    OfferId = offer.Id,
                    OfferTitle = offer.Title,
                    OldPrice = offer.Price,
                    NewPrice = nokautPrice,
                    UpdateTime = DateTime.Now
                };
                await updateCollection.InsertOneAsync(update);

                // var filter = Builders<ScrapedOffer>.Filter.Eq(x => x.Id, offer.Id);
                // var updateDefinition = Builders<ScrapedOffer>.Update.Set(x => x.Price, nokautPrice);
                // await offerCollection.UpdateOneAsync(filter, updateDefinition);
            }
        }
    }

    private async Task<string> GetWebPageHtml(string url)
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }

    private decimal ParseNokautPrice(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        

        var priceNode = doc.DocumentNode.SelectSingleNode("//span[contains(@class, 'PriceBox')]/p[contains(@class, 'Price')]");
        if (priceNode != null)
        {
            var priceText = priceNode.InnerText.Trim();
            var priceValue = decimal.Parse(priceText, NumberStyles.Currency, CultureInfo.GetCultureInfo("pl-PL"));
            return priceValue;
        }

        return 0;
    }

    
    
    
    public async Task<List<ScrapedOffer>> GetAllOffersBySearchPhrase(string searchPhrase)
    {
        try
        {
            await UpdateOffers();
            await UpdatePriceHistory();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        var database = _databaseService.ConnectToDatabase();
        var collection = database.GetCollection<ScrapedOffer>("Offers");
        var filter = Builders<ScrapedOffer>.Filter.Eq("SearchPhrase", searchPhrase);
        var cursor = await collection.FindAsync(filter);
        var offers = await cursor.ToListAsync();
        return offers;
    }
    
    public async Task UpdatePriceHistory()
    {
        var _database = _databaseService.ConnectToDatabase();
        var offerCollection = _database.GetCollection<ScrapedOffer>("Offers");
        var updateCollection = _database.GetCollection<OfferUpdate>("OfferUpdates");

        var offers = await offerCollection.Find(new BsonDocument()).ToListAsync();

        foreach (var offer in offers)
        {
            var updates = await updateCollection.Find(u => u.OfferId == offer.Id).ToListAsync();

            if (updates.Count == 0) continue;

            var priceHistory = new List<decimal>() { offer.Price };
            foreach (var updateOffer in updates.OrderBy(u => u.UpdateTime))
            {
                priceHistory.Add(@updateOffer.NewPrice);
            }

            var filter = Builders<ScrapedOffer>.Filter.Eq(o => o.Id, offer.Id);
            var update = Builders<ScrapedOffer>.Update.Set(o => o.PriceHistory, priceHistory);

            await offerCollection.UpdateOneAsync(filter, update);
        }
    }


}
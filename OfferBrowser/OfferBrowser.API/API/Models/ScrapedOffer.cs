using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OfferBrowser.API.API.Models;

public class ScrapedOffer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("title")] public string Title { get; set; }

    [BsonElement("price")] public decimal Price { get; set; }

    [BsonElement("image")] public string? ImageUrl { get; set; }
    [BsonElement("link")] public string? Link { get; set; }
    [BsonElement("searchPhrase")] public string? SearchPhrase { get; set; }
    [BsonElement("priceHistory")] public List<decimal> PriceHistory { get; set; }
}


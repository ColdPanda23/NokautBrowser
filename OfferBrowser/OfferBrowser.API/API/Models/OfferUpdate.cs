using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OfferBrowser.API.API.Models;

public class OfferUpdate
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string OfferId { get; set; }
    [BsonElement("title")]public string OfferTitle { get; set; }
    [BsonElement("oldPrice")] public decimal OldPrice { get; set; }
    [BsonElement("newPrice")]public decimal NewPrice { get; set; }
    [BsonElement("updateTime")]public DateTime UpdateTime { get; set; }
}
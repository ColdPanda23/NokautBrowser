using System.Globalization;
using HtmlAgilityPack;
using OfferBrowser.API.API.Interface;
using OfferBrowser.API.API.Models;

namespace OfferBrowser.API.API.Service;

public class ScrapeService : IScrapeService
{
    private readonly IDatabaseService _databaseService;

    public ScrapeService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<List<ScrapedOffer>> ScrapeOffer(string searchPhrase)
    {
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load($"https://www.nokaut.pl/produkt:{searchPhrase}.html");
        var titles = doc.DocumentNode.SelectNodes("//span[contains(@class, 'Title')]/a | //span[contains(@class, 'Title Multi')]/a");
        var prices = doc.DocumentNode.SelectNodes("//span[contains(@class, 'PriceBox')]/p[contains(@class, 'Price')]");
        var links = doc.DocumentNode.SelectNodes("//a[@class='ProductLink']");
        //var images = doc.DocumentNode.SelectNodes("//img[contains(@class, 'Product')]/@src");

        List<ScrapedOffer> scrapedOffers = new List<ScrapedOffer>();

        for (int i = 0; i < 3; i++)
        {
            var title = titles[i].InnerText.Trim();
            var priceString = prices[i].InnerText.Trim().Replace(",", ".").Replace(" zł", "");
            if (priceString.Contains("od"))
                priceString = priceString.Substring(priceString.IndexOf("od") + 2);
            
            var link = links[i].GetAttributeValue("href", "");
            decimal price;
            if (decimal.TryParse(priceString, NumberStyles.Any, CultureInfo.InvariantCulture, out price)
               )
            {
                Console.WriteLine($"Title: {title}");
                Console.WriteLine($"Price: {price} PLN");
                Console.WriteLine($"Link: {link}");
               // Console.WriteLine($"Image: {image}");

                ScrapedOffer scrapedOffer = new ScrapedOffer()
                {
                    Title = title,
                    Price = price,
                   // ImageUrl = image,
                    Link = link,
                    SearchPhrase = searchPhrase
                };
                scrapedOffers.Add(scrapedOffer);
            }
            else
            {
                Console.WriteLine($"Error converting price string '{priceString}' to decimal");
            }
        }

        await _databaseService.InsertScrapedOffersAsync(scrapedOffers);

        return scrapedOffers;
    }
}
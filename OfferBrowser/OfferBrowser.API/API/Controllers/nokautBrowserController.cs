using Microsoft.AspNetCore.Mvc;
using OfferBrowser.API.API.Interface;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Cors.Infrastructure;


namespace OfferBrowser.API.API.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class OffersController : ControllerBase
    {
        private readonly IScrapeService _scrapeService;
        private readonly IOfferService _offerService;

        public OffersController(IScrapeService scrapeService, IOfferService offerService)
        {
            _scrapeService = scrapeService;
            _offerService = offerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOffers()
        {
            try
            {
                var offers = await _offerService.GetAllOffers();
                return Ok(offers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{searchPhrase}")]
        public async Task<IActionResult> GetAllOffersBySearchPhrase(string searchPhrase)
        {
            try
            {
                var offers = await _offerService.GetAllOffersBySearchPhrase(searchPhrase);
                return Ok(offers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{searchPhrase}")]
        public async Task<IActionResult> ScrapeOffer(string searchPhrase)
        {
            try
            {
                var scrapedOffers = await _scrapeService.ScrapeOffer(searchPhrase);
                return Ok(scrapedOffers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
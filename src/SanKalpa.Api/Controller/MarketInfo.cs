using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanKalpa.Application.Services;

namespace SanKalpa.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketInfo : ControllerBase
    {
        private readonly ITwseDataService _twseDataService;
        private readonly ILogger<MarketInfo> _logger;

        public MarketInfo(ITwseDataService twseDataService, ILogger<MarketInfo> logger)
        {
            _twseDataService = twseDataService;
            _logger = logger;
        }

        [HttpGet("raw-data")]
        public async Task<IActionResult> GetRawDataAsync([FromQuery] string date)
        {
            try
            {
                // Validate date format (should be YYYY-MM-DD)
                if (!DateTime.TryParse(date, out DateTime parsedDate))
                {
                    return BadRequest("Invalid date format. Please use YYYY-MM-DD format.");
                }

                if (parsedDate.DayOfWeek == DayOfWeek.Saturday || parsedDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    return BadRequest("Stock market is closed on weekends.");
                }

                var rawData = await _twseDataService.GetRawStockDataAsync(date);

                if (string.IsNullOrEmpty(rawData))
                {
                    return NotFound($"No data available for date: {date}");
                }

                return Ok(rawData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching stock data");
                return StatusCode(500, "An error occurred while fetching stock data");
            }
        }
    }
}

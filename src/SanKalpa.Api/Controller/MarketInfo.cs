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
        public async Task<IActionResult> GetRawDataAsync(string date)
        {
            var rawData = await _twseDataService.GetRawStockDataAsync(date);

            if (string.IsNullOrEmpty(rawData))
            {
                _logger.LogWarning($"No data received for date: {date}");
                return NotFound();
            }

            return Ok(rawData);
        }
    }
}

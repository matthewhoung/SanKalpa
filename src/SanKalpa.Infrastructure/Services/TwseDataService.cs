using Microsoft.Extensions.Logging;
using SanKalpa.Application.Services;
using System.Net.Http;

namespace SanKalpa.Infrastructure.Services;

public class TwseDataService : ITwseDataService
{
    private readonly TwseHttpClient _twseHttpClient;
    private readonly ILogger<TwseDataService> _logger;

    public TwseDataService(TwseHttpClient twseHttpClient, ILogger<TwseDataService> logger)
    {
        _twseHttpClient = twseHttpClient;
        _logger = logger;
    }

    public async Task<string> GetRawStockDataAsync(string date)
    {
        var rawData = await _twseHttpClient.GetRawDataAsync(date);

        if (string.IsNullOrEmpty(rawData))
        {
            _logger.LogWarning($"No data received for date: {date}");
            return string.Empty;
        }

        return rawData;
    }
}

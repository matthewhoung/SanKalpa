using Microsoft.Extensions.Logging;

namespace SanKalpa.Infrastructure.Services;

public class TwseHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TwseHttpClient> _logger;

    public TwseHttpClient(HttpClient httpClient, ILogger<TwseHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GetRawDataAsync(string date)
    {
        try
        {
            string targetUrl = $"https://www.twse.com.tw/exchangeReport/MI_INDEX?response=json&date={date}&type=ALL";
            _logger.LogInformation($"Requesting data from {targetUrl}");

            var response = await _httpClient.GetAsync(targetUrl);
            var rawContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Raw Response Content: {Content}", rawContent);
                return rawContent;
            }
            else
            {
                _logger.LogError($"Failed to get data. Status Code: {response.StatusCode}");
                _logger.LogError($"Error Content: {rawContent}");
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get data from TWSE");
            return string.Empty;
        }
    }
}

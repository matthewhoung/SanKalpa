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
        ConfigureHttpClient(httpClient);
    }

    private void ConfigureHttpClient(HttpClient client)
    {
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
        client.DefaultRequestHeaders.Add("Accept-Language", "zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7");
        client.DefaultRequestHeaders.Add("Connection", "keep-alive");
        client.DefaultRequestHeaders.Add("Host", "www.twse.com.tw");
        client.DefaultRequestHeaders.Add("Referer", "https://www.twse.com.tw/zh/page/trading/exchange/MI_INDEX.html");
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36");
        client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
    }

    public async Task<string> GetRawDataAsync(string date)
    {
        try
        {
            // Format date to match YYYYMMDD format
            string formattedDate = date.Replace("-", "");

            string targetUrl = $"https://www.twse.com.tw/exchangeReport/MI_INDEX?response=json&date={formattedDate}&type=ALL";
            _logger.LogInformation($"Requesting data from {targetUrl}");

            await Task.Delay(5000); // 5 seconds delay

            var response = await _httpClient.GetAsync(targetUrl);
            var rawContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully received response from TWSE");
                _logger.LogDebug("Raw Response Content: {Content}", rawContent);
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

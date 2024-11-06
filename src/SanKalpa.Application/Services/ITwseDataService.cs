namespace SanKalpa.Application.Services;

public interface ITwseDataService
{
    Task<string> GetRawStockDataAsync(string date);
}

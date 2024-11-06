namespace SanKalpa.Domain.Stocks.Repositories;

public interface ITwseDataService
{
    Task<string> GetStockDataAsync();
}

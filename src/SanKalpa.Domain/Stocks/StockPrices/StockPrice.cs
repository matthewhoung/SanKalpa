using SanKalpa.Domain.Abstrations;

namespace SanKalpa.Domain.Stocks.StockPrices;

public sealed class StockPrice : Entity
{
    public string Symbol { get; private set; }
    public int Tradesaction { get; private set; }
    public int TradeVolume { get; private set; }
    public int TradeValue { get; private set; }
    public float Price { get; private set; }
}

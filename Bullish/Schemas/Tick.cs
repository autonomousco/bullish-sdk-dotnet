namespace Bullish.Schemas;

public record Tick
{
    public DateTime CreatedAtDatetime { get; init; }
    public string CreatedAtTimestamp { get; init; } = string.Empty;
    public decimal High { get; init; }
    public decimal Low { get; init; }
    public decimal BestBid { get; init; }
    public decimal BidVolume { get; init; }
    public decimal BestAsk { get; init; }
    public decimal AskVolume { get; init; }
    public decimal Vwap { get; init; }
    public decimal Open { get; init; }
    public decimal Close { get; init; }
    public decimal Last { get; init; }
    public decimal Change { get; init; }
    public decimal Percentage { get; init; }
    public decimal Average { get; init; }
    public decimal BaseVolume { get; init; }
    public decimal QuoteVolume { get; init; }
    public decimal BancorPrice { get; init; }
    public DateTime LastTradeDatetime { get; init; }
    public string LastTradeTimestamp { get; init; } = string.Empty;
    public decimal LastTradeQuantity { get; init; }
    public List<AmmData> AmmData { get; init; } = new();
}
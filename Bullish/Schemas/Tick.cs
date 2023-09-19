namespace Bullish.Schemas;

public record Tick
{
    public required DateTime CreatedAtDatetime { get; init; }
    public required string CreatedAtTimestamp { get; init; }
    public required decimal High { get; init; }
    public required decimal Low { get; init; }
    public required decimal BestBid { get; init; }
    public required decimal BidVolume { get; init; }
    public required decimal BestAsk { get; init; }
    public required decimal AskVolume { get; init; }
    public required decimal Vwap { get; init; }
    public required decimal Open { get; init; }
    public required decimal Close { get; init; }
    public required decimal Last { get; init; }
    public required decimal Change { get; init; }
    public required decimal Percentage { get; init; }
    public required decimal Average { get; init; }
    public required decimal BaseVolume { get; init; }
    public required decimal QuoteVolume { get; init; }
    public required decimal BancorPrice { get; init; }
    public required DateTime LastTradeDatetime { get; init; }
    public required string LastTradeTimestamp { get; init; }
    public required decimal LastTradeQuantity { get; init; }
    public required List<AmmData> AmmData { get; init; }
}
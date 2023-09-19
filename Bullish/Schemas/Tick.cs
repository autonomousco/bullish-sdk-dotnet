namespace Bullish.Schemas;

public record Tick
{
    public required DateTime CreatedAtDatetime { get; init; }
    public required string CreatedAtTimestamp { get; init; }
    public required string High { get; init; }
    public required string Low { get; init; }
    public required string BestBid { get; init; }
    public required string BidVolume { get; init; }
    public required string BestAsk { get; init; }
    public required string AskVolume { get; init; }
    public required string Vwap { get; init; }
    public required string Open { get; init; }
    public required string Close { get; init; }
    public required string Last { get; init; }
    public required string Change { get; init; }
    public required string Percentage { get; init; }
    public required string Average { get; init; }
    public required string BaseVolume { get; init; }
    public required string QuoteVolume { get; init; }
    public required string BancorPrice { get; init; }
    public required DateTime LastTradeDatetime { get; init; }
    public required string LastTradeTimestamp { get; init; }
    public required string LastTradeQuantity { get; init; }
    public required List<AmmData> AmmData { get; init; }
}
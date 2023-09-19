namespace Bullish.Schemas;

public record MarketCandle
{
    public required string Open { get; init; }
    public required string High { get; init; }
    public required string Low { get; init; }
    public required string Close { get; init; }
    public required string Volume { get; init; }
    public required string CreatedAtTimestamp { get; init; }
    public required DateTime CreatedAtDatetime { get; init; }
}
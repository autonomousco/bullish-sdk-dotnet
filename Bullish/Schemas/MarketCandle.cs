namespace Bullish.Schemas;

public record MarketCandle
{
    public required decimal Open { get; init; }
    public required decimal High { get; init; }
    public required decimal Low { get; init; }
    public required decimal Close { get; init; }
    public required decimal Volume { get; init; }
    public required string CreatedAtTimestamp { get; init; }
    public required DateTime CreatedAtDatetime { get; init; }
}
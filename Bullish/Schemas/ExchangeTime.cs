namespace Bullish.Schemas;

public record ExchangeTime
{
    public long Timestamp { get; init; }
    public DateTime DateTime { get; init; }
}
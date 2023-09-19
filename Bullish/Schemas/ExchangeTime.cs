namespace Bullish.Schemas;

public record ExchangeTime
{
    public required long Timestamp { get; init; }
    public required DateTime DateTime { get; init; }
}
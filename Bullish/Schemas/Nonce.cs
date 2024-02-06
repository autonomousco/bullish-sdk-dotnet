namespace Bullish.Schemas;

public record Nonce
{
    public long LowerBound { get; init; }
    public long UpperBound { get; init; }
}
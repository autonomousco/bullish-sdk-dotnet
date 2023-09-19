namespace Bullish.Schemas;

public record Ask
{
    public required string Price { get; init; }
    public required string PriceLevelQuantity { get; init; }
}
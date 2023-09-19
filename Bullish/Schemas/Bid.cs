namespace Bullish.Schemas;

public record Bid
{
    public required string Price { get; init; }
    public required string PriceLevelQuantity { get; init; }
}
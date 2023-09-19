namespace Bullish.Schemas;

public record Bid
{
    public required decimal Price { get; init; }
    public required decimal PriceLevelQuantity { get; init; }
}
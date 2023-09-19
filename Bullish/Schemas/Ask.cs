namespace Bullish.Schemas;

public record Ask
{
    public required decimal Price { get; init; }
    public required decimal PriceLevelQuantity { get; init; }
}
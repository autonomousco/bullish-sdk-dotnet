namespace Bullish.Schemas;

public record MarketTrade
{
    public required string TradeId { get; init; }
    public required string Symbol { get; init; }
    public required string Price { get; init; }
    public required string Quantity { get; init; }
    public required string Side { get; init; }
    public required bool IsTaker { get; init; }
    public required DateTime CreatedAtDatetime { get; init; }
    public required string CreatedAtTimestamp { get; init; }
}
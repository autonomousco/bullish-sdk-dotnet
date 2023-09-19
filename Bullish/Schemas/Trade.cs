namespace Bullish.Schemas;

public record Trade
{
    public required string TradeId { get; init; }
    public required string OrderId { get; init; }
    public required string Symbol { get; init; }
    public required decimal Price { get; init; }
    public required decimal Quantity { get; init; }
    public required decimal BaseFee { get; init; }
    public required decimal QuoteFee { get; init; }
    public required string Side { get; init; }
    public required bool IsTaker { get; init; }
    public required DateTime CreatedAtDatetime { get; init; }
    public required string CreatedAtTimestamp { get; init; }
}
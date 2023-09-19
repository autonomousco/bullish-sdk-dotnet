namespace Bullish.Schemas;

public record Order
{
    public required string Handle { get; init; }
    public required string OrderId { get; init; }
    public required string Symbol { get; init; }
    public required decimal Price { get; init; }
    public required decimal AverageFillPrice { get; init; }
    public required decimal StopPrice { get; init; }
    public required bool Margin { get; init; }
    public required decimal Quantity { get; init; }
    public required decimal QuantityFilled { get; init; }
    public required decimal BaseFee { get; init; }
    public required decimal QuoteFee { get; init; }
    public required decimal BorrowedQuantity { get; init; }
    public required bool IsLiquidation { get; init; }
    public required string Side { get; init; }
    public required string Type { get; init; }
    public required string TimeInForce { get; init; }
    public required string Status { get; init; }
    public required string StatusReason { get; init; }
    public required string StatusReasonCode { get; init; }
    public required DateTime CreatedAtDatetime { get; init; }
    public required string CreatedAtTimestamp { get; init; }
}
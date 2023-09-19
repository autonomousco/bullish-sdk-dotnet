namespace Bullish.Schemas;

public record Order
{
    public required string Handle { get; init; }
    public required string OrderId { get; init; }
    public required string Symbol { get; init; }
    public required string Price { get; init; }
    public required string AverageFillPrice { get; init; }
    public required string StopPrice { get; init; }
    public required bool Margin { get; init; }
    public required string Quantity { get; init; }
    public required string QuantityFilled { get; init; }
    public required string BaseFee { get; init; }
    public required string QuoteFee { get; init; }
    public required string BorrowedQuantity { get; init; }
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
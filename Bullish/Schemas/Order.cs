namespace Bullish.Schemas;

public record Order
{
    public string ClientOrderId { get; init; } = string.Empty;
    public string OrderId { get; init; } = string.Empty;
    public string Symbol { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal? AverageFillPrice { get; init; }
    public decimal? StopPrice { get; init; }
    public bool AllowBorrow { get; init; }
    public decimal Quantity { get; init; }
    public decimal QuantityFilled { get; init; }
    public decimal BaseFee { get; init; }
    public decimal QuoteFee { get; init; }
    public decimal? BorrowedBaseQuantity { get; init; }
    public decimal? BorrowedQuoteQuantity { get; init; }
    public bool IsLiquidation { get; init; }
    public OrderSide Side { get; init; }
    public string Type { get; init; } = string.Empty;
    public OrderTimeInForce TimeInForce { get; init; }
    public string Status { get; init; } = string.Empty;
    public string StatusReason { get; init; } = string.Empty;
    public string StatusReasonCode { get; init; } = string.Empty;
    public DateTime CreatedAtDatetime { get; init; }
    public string CreatedAtTimestamp { get; init; } = string.Empty;
}
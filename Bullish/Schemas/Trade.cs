namespace Bullish.Schemas;

public record Trade
{
    public string TradeId { get; init; } = string.Empty;
    public string OrderId { get; init; } = string.Empty;
    public string Symbol { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal Quantity { get; init; }
    public decimal BaseFee { get; init; }
    public decimal QuoteFee { get; init; }
    public decimal QuoteAmount { get; init; }
    public string Side { get; init; } = string.Empty;
    public bool IsTaker { get; init; }
    public DateTime CreatedAtDatetime { get; init; }
    public string CreatedAtTimestamp { get; init; } = string.Empty;
}
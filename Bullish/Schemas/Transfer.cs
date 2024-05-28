namespace Bullish.Schemas;

public record Transfer
{
    public string RequestId { get; init; } = string.Empty;
    public string ToTradingAccountId { get; init; } = string.Empty;
    public string FromTradingAccountId { get; init; } = string.Empty;
    public string AssetSymbol { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public TransferStatus Status { get; init; }
    public string StatusReasonCode { get; init; } = string.Empty;
    public string StatusReason { get; init; } = string.Empty;
    public string CreatedAtTimestamp { get; init; } = string.Empty;
    public DateTime CreatedAtDatetime { get; init; }
}
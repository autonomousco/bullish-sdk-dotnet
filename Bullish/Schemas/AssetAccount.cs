namespace Bullish.Schemas;

public record AssetAccount
{
    public string TradingAccountId { get; init; } = string.Empty;
    public string AssetId { get; init; } = string.Empty;
    public string AssetSymbol { get; init; } = string.Empty;
    public decimal AvailableQuantity { get; init; }
    public decimal BorrowedQuantity { get; init; }
    public decimal LockedQuantity { get; init; }
    public decimal LoanedQuantity { get; init; }
    public DateTime UpdatedAtDatetime { get; init; }
    public string UpdatedAtTimestamp { get; init; } = string.Empty;
}
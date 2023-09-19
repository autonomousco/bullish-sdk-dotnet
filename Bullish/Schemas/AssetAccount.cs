namespace Bullish.Schemas;

public record AssetAccount
{
    public required string TradingAccountId { get; init; }
    public required string AssetId { get; init; }
    public required string AssetSymbol { get; init; }
    public required decimal AvailableQuantity { get; init; }
    public required decimal BorrowedQuantity { get; init; }
    public required decimal LockedQuantity { get; init; }
    public required decimal LoanedQuantity { get; init; }
    public required DateTime UpdatedAtDatetime { get; init; }
    public required string UpdatedAtTimestamp { get; init; }
}
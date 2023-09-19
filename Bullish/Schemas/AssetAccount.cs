namespace Bullish.Schemas;

public record AssetAccount
{
    public required string TradingAccountId { get; init; }
    public required string AssetId { get; init; }
    public required string AssetSymbol { get; init; }
    public required string AvailableQuantity { get; init; }
    public required string BorrowedQuantity { get; init; }
    public required string LockedQuantity { get; init; }
    public required string LoanedQuantity { get; init; }
    public required DateTime UpdatedAtDatetime { get; init; }
    public required string UpdatedAtTimestamp { get; init; }
}
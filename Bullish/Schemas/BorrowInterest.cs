namespace Bullish.Schemas;

public record BorrowInterest
{
    public required string AssetId { get; init; }
    public required string AssetSymbol { get; init; }
    public required string BorrowedQuantity { get; init; }
    public required string TotalBorrowedQuantity { get; init; }
    public required DateTime CreatedAtDatetime { get; init; }
    public required string CreatedAtTimestamp { get; init; }
}
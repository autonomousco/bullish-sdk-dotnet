namespace Bullish.Schemas;

public record BorrowInterest
{
    public required string AssetId { get; init; }
    public required string AssetSymbol { get; init; }
    public required decimal BorrowedQuantity { get; init; }
    public required decimal TotalBorrowedQuantity { get; init; }
    public required DateTime CreatedAtDatetime { get; init; }
    public required string CreatedAtTimestamp { get; init; }
}
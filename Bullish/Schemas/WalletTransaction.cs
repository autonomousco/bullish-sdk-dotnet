namespace Bullish.Schemas;

public record WalletTransaction
{
    public required string CustodyTransactionId { get; init; }
    public required string Direction { get; init; }
    public required string Quantity { get; init; }
    public required string Symbol { get; init; }
    public required string Network { get; init; }
    public required string Fee { get; init; }
    public required string Memo { get; init; }
    public required DateTime CreatedAtDateTime { get; init; }
    public required string Status { get; init; }
    public required TransactionDetails TransactionDetails { get; init; }
}
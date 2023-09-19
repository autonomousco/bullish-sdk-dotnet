namespace Bullish.Schemas;

public record TransactionDetails
{
    public required string Address { get; init; }
    public required string BlockchainTxId { get; init; }
    public required string SwiftUetr { get; init; }
}
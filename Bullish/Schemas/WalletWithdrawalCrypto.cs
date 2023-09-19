namespace Bullish.Schemas;

public record WalletWithdrawalCrypto
{
    public required string Network { get; init; }
    public required string Symbol { get; init; }
    public required string Address { get; init; }
    public required string Fee { get; init; }
    public required string Memo { get; init; }
    public required string Label { get; init; }
    public required string DestinationId { get; init; }
}
namespace Bullish.Schemas;

public record WalletDepositCrypto
{
    public required string Network { get; init; }
    public required string Symbol { get; init; }
    public required string Address { get; init; }
}
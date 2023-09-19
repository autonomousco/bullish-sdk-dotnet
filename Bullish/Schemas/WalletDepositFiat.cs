namespace Bullish.Schemas;

public record WalletDepositFiat
{
    public required string Network { get; init; }
    public required string Symbol { get; init; }
    public required string AccountNumber { get; init; }
    public required string Name { get; init; }
    public required string PhysicalAddress { get; init; }
    public required string Memo { get; init; }
    public required Bank Bank { get; init; }
}
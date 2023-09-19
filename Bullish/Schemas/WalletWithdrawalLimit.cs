namespace Bullish.Schemas;

public record WalletWithdrawalLimit
{
    public required string Symbol { get; init; }
    public required string Available { get; init; }
    public required string TwentyFourHour { get; init; }
}
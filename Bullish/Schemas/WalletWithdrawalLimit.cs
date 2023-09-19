namespace Bullish.Schemas;

public record WalletWithdrawalLimit
{
    public required string Symbol { get; init; }
    public required decimal Available { get; init; }
    public required decimal TwentyFourHour { get; init; }
}
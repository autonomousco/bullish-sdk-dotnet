namespace Bullish.Schemas;

public record WalletWithdrawalLimit
{
    public string Symbol { get; init; } = string.Empty;
    public decimal Available { get; init; }
    public decimal TwentyFourHour { get; init; }
}
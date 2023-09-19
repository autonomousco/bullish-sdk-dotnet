namespace Bullish.Schemas;

public record TradingAccount
{
    public required string IsBorrowing { get; init; }
    public required string IsLending { get; init; }
    public required decimal MakerFee { get; init; }
    public required decimal TakerFee { get; init; }
    public required decimal MaxInitialLeverage { get; init; }
    public required string TradingAccountId { get; init; }
    public required string TradingAccountName { get; init; }
    public required string TradingAccountDescription { get; init; }
    public required string IsPrimaryAccount { get; init; }
    public required string RateLimitToken { get; init; }
    public required string IsDefaulted { get; init; }
}
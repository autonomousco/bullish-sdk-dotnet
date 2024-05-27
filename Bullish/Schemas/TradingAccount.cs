namespace Bullish.Schemas;

public record TradingAccount
{
    public bool IsBorrowing { get; init; }
    public bool IsLending { get; init; }
    public decimal? MakerFee { get; init; }
    public decimal? TakerFee { get; init; }
    public decimal MaxInitialLeverage { get; init; }
    public string TradingAccountId { get; init; } = string.Empty;
    public string TradingAccountName { get; init; } = string.Empty;
    public string TradingAccountDescription { get; init; } = string.Empty;
    public bool IsPrimaryAccount { get; init; }
    public string RateLimitToken { get; init; } = string.Empty;
    public bool IsDefaulted { get; init; }
    public decimal RiskLimitUsd { get; init; }
    public decimal TotalBorrowedUsd { get; init; }
    public decimal TotalCollateralUsd { get; init; }
    public decimal InitialMarginUsd { get; init; }
    public decimal WarningMarginUsd { get; init; }
    public decimal LiquidationMarginUsd { get; init; }
    public decimal FullLiquidationMarginUsd { get; init; }
    public decimal DefaultedMarginUsd { get; init; }
    public string EndCustomerId { get; init; } = string.Empty;
}

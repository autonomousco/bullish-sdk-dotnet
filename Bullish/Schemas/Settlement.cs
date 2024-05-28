namespace Bullish.Schemas;

public record Settlement
{
    public string TradingAccountId { get; init; } = string.Empty;
    public string Symbol { get; init; } = string.Empty;
    public OrderSide Side { get; init; }
    public decimal SettlementQuantity { get; init; }
    public decimal DeltaTradingQuantity { get; init; }
    public decimal MtmPnl { get; init; }
    public decimal FundingPnl { get; init; }
    public decimal SettlementMarkPrice { get; init; }
    public decimal SettlementIndexPrice { get; init; }
    public decimal SettlementFundingRate { get; init; }
    public DateTime SettlementDatetime { get; init; }
    public string SettlementTimestamp { get; init; } = string.Empty;
}
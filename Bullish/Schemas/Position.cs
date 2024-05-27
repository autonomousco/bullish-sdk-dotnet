namespace Bullish.Schemas;

public record Position
{
    public string TradingAccountId { get; init; } = string.Empty;
    public string Symbol { get; init; } = string.Empty;
    public string Side { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public decimal Notional { get; init; }
    public decimal EntryNotional { get; init; }
    public decimal MtmPnl { get; init; }
    public decimal ReportedMtmPnl { get; init; }
    public decimal ReportedFundingPnl { get; init; }
    public decimal RealizedPnl { get; init; }
    public string SettlementAssetSymbol { get; init; } = string.Empty;
    public DateTime CreatedAtDatetime { get; init; }
    public string CreatedAtTimestamp { get; init; } = string.Empty;
    public DateTime UpdatedAtDatetime { get; init; }
    public string UpdatedAtTimestamp { get; init; } = string.Empty;
}
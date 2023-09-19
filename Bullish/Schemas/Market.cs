namespace Bullish.Schemas;

public record Market
{
    public string MarketId { get; init; } = string.Empty;
    public string Symbol { get; init; } = string.Empty;
    public string BaseSymbol { get; init; } = string.Empty;
    public string QuoteSymbol { get; init; } = string.Empty;
    public string QuoteAssetId { get; init; } = string.Empty;
    public string BaseAssetId { get; init; } = string.Empty;
    public int QuotePrecision { get; init; }
    public int BasePrecision { get; init; }
    public int PricePrecision { get; init; }
    public int QuantityPrecision { get; init; }
    public int CostPrecision { get; init; }
    public decimal MinQuantityLimit { get; init; }
    public decimal MaxQuantityLimit { get; init; }
    public decimal? MaxPriceLimit { get; init; }
    public decimal? MinPriceLimit { get; init; }
    public decimal? MaxCostLimit { get; init; }
    public decimal? MinCostLimit { get; init; }
    public string TimeZone { get; init; } = string.Empty;
    public decimal TickSize { get; init; }
    public decimal LiquidityTickSize { get; init; }
    public int LiquidityPrecision { get; init; }
    public int MakerFee { get; init; }
    public int TakerFee { get; init; }
    public List<string> OrderTypes { get; init; } = new();
    public bool SpotTradingEnabled { get; init; }
    public bool MarginTradingEnabled { get; init; }
    public bool MarketEnabled { get; init; }
    public bool CreateOrderEnabled { get; init; }
    public bool CancelOrderEnabled { get; init; }
    public List<FeeTier> FeeTiers { get; init; } = new();
}
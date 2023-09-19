namespace Bullish.Schemas;

public record Market
{
    public required string MarketId { get; init; }
    public required string Symbol { get; init; }
    public required string BaseSymbol { get; init; }
    public required string QuoteSymbol { get; init; }
    public required string QuoteAssetId { get; init; }
    public required string BaseAssetId { get; init; }
    public required int QuotePrecision { get; init; }
    public required int BasePrecision { get; init; }
    public required int PricePrecision { get; init; }
    public required int QuantityPrecision { get; init; }
    public required int CostPrecision { get; init; }
    public required decimal MinQuantityLimit { get; init; }
    public required decimal MaxQuantityLimit { get; init; }
    public required decimal? MaxPriceLimit { get; init; }
    public required decimal? MinPriceLimit { get; init; }
    public required decimal? MaxCostLimit { get; init; }
    public required decimal? MinCostLimit { get; init; }
    public required string TimeZone { get; init; }
    public required decimal TickSize { get; init; }
    public required decimal LiquidityTickSize { get; init; }
    public required int LiquidityPrecision { get; init; }
    public required int MakerFee { get; init; }
    public required int TakerFee { get; init; }
    public required List<string> OrderTypes { get; init; }
    public required bool SpotTradingEnabled { get; init; }
    public required bool MarginTradingEnabled { get; init; }
    public required bool MarketEnabled { get; init; }
    public required bool CreateOrderEnabled { get; init; }
    public required bool CancelOrderEnabled { get; init; }
    public required List<FeeTier> FeeTiers { get; init; }
}
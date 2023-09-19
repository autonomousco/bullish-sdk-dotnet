using System.Text.Json.Serialization;

namespace Bullish.Schemas;

public record AmmInstruction
{
    public required string LiquidityId { get; init; }
    public required string Symbol { get; init; }
    public required decimal BaseFee { get; init; }
    public required decimal QuoteFee { get; init; }
    public required string Status { get; init; }
    public required string StatusReason { get; init; }
    public required int StatusReasonCode { get; init; }
    public required DateTime CreatedAtDatetime { get; init; }
    public required string CreatedAtTimestamp { get; init; }
    [JsonPropertyName("24HrApy")] public required decimal TwentyFourHrApy { get; init; }
    [JsonPropertyName("24HrYieldEarn")] public required decimal TwentyFourHrYieldEarn { get; init; }
    public required decimal Apy { get; init; }
    public required decimal BaseCurrentQuantity { get; init; }
    public required decimal BaseInvestQuantity { get; init; }
    public required decimal BasePrice { get; init; }
    public required decimal BaseWithdrawQuantity { get; init; }
    public required decimal CurrentValue { get; init; }
    public required bool DislocationEnabled { get; init; }
    public required string FeeTierId { get; init; }
    public required decimal FinalValue { get; init; }
    public required decimal ImpermanentLoss { get; init; }
    public required decimal InitialBasePrice { get; init; }
    public required decimal InitialQuotePrice { get; init; }
    public required decimal InitialValue { get; init; }
    public required decimal LowerBound { get; init; }
    public required decimal Price { get; init; }
    public required decimal QuoteCurrentQuantity { get; init; }
    public required decimal QuoteInvestQuantity { get; init; }
    public required decimal QuotePrice { get; init; }
    public required decimal QuoteWithdrawQuantity { get; init; }
    public required string RequestId { get; init; }
    public required decimal StaticSpreadFee { get; init; }
    public required DateTime UpdatedAtDatetime { get; init; }
    public required string UpdatedAtTimestamp { get; init; }
    public required decimal UpperBound { get; init; }
    public required decimal YieldEarn { get; init; }
}
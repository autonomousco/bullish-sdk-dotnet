using System.Text.Json.Serialization;

namespace Bullish.Schemas;

public record AmmInstruction
{
    public required string LiquidityId { get; init; }
    public required string Symbol { get; init; }
    public required string BaseFee { get; init; }
    public required string QuoteFee { get; init; }
    public required string Status { get; init; }
    public required string StatusReason { get; init; }
    public required int StatusReasonCode { get; init; }
    public required DateTime CreatedAtDatetime { get; init; }
    public required string CreatedAtTimestamp { get; init; }
    [JsonPropertyName("24HrApy")] public required string TwentyFourHrApy { get; init; }
    [JsonPropertyName("24HrYieldEarn")] public required string TwentyFourHrYieldEarn { get; init; }
    public required string Apy { get; init; }
    public required string BaseCurrentQuantity { get; init; }
    public required string BaseInvestQuantity { get; init; }
    public required string BasePrice { get; init; }
    public required string BaseWithdrawQuantity { get; init; }
    public required string CurrentValue { get; init; }
    public required bool DislocationEnabled { get; init; }
    public required string FeeTierId { get; init; }
    public required string FinalValue { get; init; }
    public required string ImpermanentLoss { get; init; }
    public required string InitialBasePrice { get; init; }
    public required string InitialQuotePrice { get; init; }
    public required string InitialValue { get; init; }
    public required string LowerBound { get; init; }
    public required string Price { get; init; }
    public required string QuoteCurrentQuantity { get; init; }
    public required string QuoteInvestQuantity { get; init; }
    public required string QuotePrice { get; init; }
    public required string QuoteWithdrawQuantity { get; init; }
    public required string RequestId { get; init; }
    public required string StaticSpreadFee { get; init; }
    public required DateTime UpdatedAtDatetime { get; init; }
    public required string UpdatedAtTimestamp { get; init; }
    public required string UpperBound { get; init; }
    public required string YieldEarn { get; init; }
}
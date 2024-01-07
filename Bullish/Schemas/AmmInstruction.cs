using System.Text.Json.Serialization;

namespace Bullish.Schemas;

public record AmmInstruction
{
    public string InstructionId { get; init; } = string.Empty;
    public string Symbol { get; init; } = string.Empty;
    public decimal BaseFee { get; init; }
    public decimal QuoteFee { get; init; }
    public string Status { get; init; } = string.Empty;
    public string StatusReason { get; init; } = string.Empty;
    public int StatusReasonCode { get; init; }
    public DateTime CreatedAtDatetime { get; init; }
    public string CreatedAtTimestamp { get; init; } = string.Empty;
    [JsonPropertyName("24HrApy")] public decimal TwentyFourHrApy { get; init; }
    [JsonPropertyName("24HrYieldEarn")] public decimal TwentyFourHrYieldEarn { get; init; }
    public decimal Apy { get; init; }
    public decimal BaseCurrentQuantity { get; init; }
    public decimal BaseInvestQuantity { get; init; }
    public decimal BasePrice { get; init; }
    public decimal BaseWithdrawQuantity { get; init; }
    public decimal CurrentValue { get; init; }
    public bool DislocationEnabled { get; init; }
    public string FeeTierId { get; init; } = string.Empty;
    public decimal FinalValue { get; init; }
    public decimal ImpermanentLoss { get; init; }
    public decimal InitialBasePrice { get; init; }
    public decimal InitialQuotePrice { get; init; }
    public decimal InitialValue { get; init; }
    public decimal LowerBound { get; init; }
    public decimal Price { get; init; }
    public decimal QuoteCurrentQuantity { get; init; }
    public decimal QuoteInvestQuantity { get; init; }
    public decimal QuotePrice { get; init; }
    public decimal QuoteWithdrawQuantity { get; init; }
    public string RequestId { get; init; } = string.Empty;
    public decimal StaticSpreadFee { get; init; }
    public DateTime UpdatedAtDatetime { get; init; }
    public string UpdatedAtTimestamp { get; init; } = string.Empty;
    public decimal UpperBound { get; init; }
    public decimal YieldEarn { get; init; }
}
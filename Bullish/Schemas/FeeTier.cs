namespace Bullish.Schemas;

public record FeeTier
{
    public required string FeeTierId { get; init; }
    public required decimal StaticSpreadFee { get; init; }
    public required bool IsDislocationEnabled { get; init; }
}
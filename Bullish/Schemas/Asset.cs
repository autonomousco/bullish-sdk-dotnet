namespace Bullish.Schemas;

public record Asset
{
    public required string AssetId { get; init; }
    public required string Symbol { get; init; }
    public required int Precision { get; init; }
    public required decimal MinBalanceInterest { get; init; }
    public required decimal MinFee { get; init; }
    public required decimal Apr { get; init; }
    public required decimal CollateralRating { get; init; }
    public required decimal MaxBorrow { get; init; }
}
namespace Bullish.Schemas;

public record Asset
{
    public required string AssetId { get; init; }
    public required string Symbol { get; init; }
    public required string Precision { get; init; }
    public required string MinBalanceInterest { get; init; }
    public required string MinFee { get; init; }
    public required string Apr { get; init; }
    public required string CollateralRating { get; init; }
    public required string MaxBorrow { get; init; }
}
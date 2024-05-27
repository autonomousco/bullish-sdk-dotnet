namespace Bullish.Schemas;

public record Asset
{
    public string AssetId { get; init; } = string.Empty;
    public string Symbol { get; init; } = string.Empty;
    public int Precision { get; init; }
    public decimal MinBalanceInterest { get; init; }
    public decimal MinFee { get; init; }
    public decimal Apr { get; init; }
    public decimal CollateralRating { get; init; }
    public decimal MaxBorrow { get; init; }
    public decimal TotalOfferedLoanQuantity { get; init; }
    public decimal LoanBorrowedQuantity { get; init; }
}

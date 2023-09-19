namespace Bullish.Schemas;

public record AmmData
{
    public required string FeeTierId { get; init; }
    public required string BidSpreadFee { get; init; }
    public required string AskSpreadFee { get; init; }
    public required string BaseReservesQuantity { get; init; }
    public required string QuoteReservesQuantity { get; init; }
    public required string CurrentPrice { get; init; }
}
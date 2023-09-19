namespace Bullish.Schemas;

public record AmmData
{
    public required string FeeTierId { get; init; }
    public required decimal BidSpreadFee { get; init; }
    public required decimal AskSpreadFee { get; init; }
    public required decimal BaseReservesQuantity { get; init; }
    public required decimal QuoteReservesQuantity { get; init; }
    public required decimal CurrentPrice { get; init; }
}
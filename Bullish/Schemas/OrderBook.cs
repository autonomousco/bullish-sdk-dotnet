namespace Bullish.Schemas;

public record OrderBook
{
    public required List<Bid> Bids { get; init; }
    public required List<Ask> Asks { get; init; }
    public required DateTime Datetime { get; init; }
    public required string Timestamp { get; init; }
    public required int SequenceNumber { get; init; }
}
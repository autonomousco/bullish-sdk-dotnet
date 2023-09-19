namespace Bullish.Schemas;

public record OrderBook
{
    public List<Bid> Bids { get; init; } = new();
    public List<Ask> Asks { get; init; } = new();
    public DateTime Datetime { get; init; }
    public string Timestamp { get; init; } = string.Empty;
    public int SequenceNumber { get; init; }
}
namespace Bullish.Schemas;

public record CancelAllOrders
{
    public required string Message { get; init; }
    public required string RequestId { get; init; }
}
namespace Bullish.Schemas;

public record CancelAllOrders
{
    public string Message { get; init; } = string.Empty;
    public string RequestId { get; init; } = string.Empty;
}
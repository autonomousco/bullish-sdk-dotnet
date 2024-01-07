namespace Bullish.Schemas;

public record CancelOrder
{
    public string Message { get; init; } = string.Empty;
    public string RequestId { get; init; } = string.Empty;
    public string OrderId { get; init; } = string.Empty;
}
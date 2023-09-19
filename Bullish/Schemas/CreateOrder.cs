namespace Bullish.Schemas;

public record CreateOrder
{
    public string Message { get; init; } = string.Empty;
    public string RequestId { get; init; } = string.Empty;
    public string OrderId { get; init; } = string.Empty;
    public bool Test { get; init; }
}
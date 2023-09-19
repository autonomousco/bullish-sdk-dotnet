namespace Bullish.Schemas;

public record CreateOrder
{
    public required string Message { get; init; }
    public required string RequestId { get; init; }
    public required string OrderId { get; init; }
    public required bool Test { get; init; }
}
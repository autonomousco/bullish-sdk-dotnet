namespace Bullish;

public record CancelAllOrdersResponse
{
    public required string Message { get; init; }
    public required string RequestId { get; init; }

    // Returned but unused
    // public required object OrderId { get; init; }
    // public required object Handle { get; init; }
    // public required bool Test { get; init; }
}

public record CreateOrderResponse
{
    public required string Message { get; init; }
    public required string RequestId { get; init; }
    public required string OrderId { get; init; }
    public required bool Test { get; init; }
}

public record LogoutResponse;
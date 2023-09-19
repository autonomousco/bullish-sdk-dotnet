namespace Bullish.Schemas;

public record IntermediaryBank
{
    public required string Name { get; init; }
    public required string PhysicalAddress { get; init; }
    public required string RoutingCode { get; init; }
}
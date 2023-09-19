namespace Bullish.Schemas;

public record Login
{
    public required string Authorizer { get; init; }
    public required string OwnerAuthorizer { get; init; }
    public required string Token { get; init; }
}
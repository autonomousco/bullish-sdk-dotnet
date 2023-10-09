namespace Bullish.Schemas;

public record Login
{
    public string Authorizer { get; init; } = string.Empty;
    // public string OwnerAuthorizer { get; init; } = string.Empty;s
    public string Token { get; init; } = string.Empty;
}
namespace Bullish.Api.Client.BxClient;

public record BxAuthToken
{
    public required string Jwt { get; init; }
    public required DateTime Expiration { get; init; }

    public static BxAuthToken New(string jwt) => new()
    {
        Jwt = jwt,
        Expiration = DateTime.UtcNow.AddHours(23),
    };

    public static BxAuthToken Empty => new()
    {
        Jwt = string.Empty,
        Expiration = DateTime.MinValue,
    };
    
    public bool IsValid => DateTime.UtcNow < Expiration;
}
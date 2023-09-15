namespace Bullish.Api.Client.BxClient;

public record BxAuthToken(string Jwt, DateTime Expiration)
{
    private BxAuthToken() : this(string.Empty, DateTime.MinValue) { }
    
    public BxAuthToken(string jwt) : this(jwt, DateTime.UtcNow.AddHours(23)) { }

    public static BxAuthToken Empty => new();
    
    public bool IsValid => DateTime.UtcNow < Expiration;
}
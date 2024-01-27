namespace Bullish.Internals;

internal sealed record AuthToken(string Jwt, DateTime Expiration)
{
    private AuthToken() : this(string.Empty, DateTime.MinValue) { }
    
    public AuthToken(string jwt) : this(jwt, DateTime.UtcNow.AddHours(23)) { }

    public static AuthToken Empty => new();
    
    public bool IsValid => DateTime.UtcNow < Expiration;
}
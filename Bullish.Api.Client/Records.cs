using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ApiTester")]

namespace Bullish.Api.Client;

public record BxMetadata
{
    public required string UserId { get; init; }
    public required string PublicKey { get; init; }
    public required string CredentialId { get; init; }
}

public record LoginPayload
{
    public required string UserId { get; init; }
    public required long Nonce { get; init; }
    public required long ExpirationTime { get; init; }
    public required bool BiometricsUsed { get; init; }
    public required string? SessionKey { get; init; }
}

public record Login
{
    public required string PublicKey { get; init; }
    public required string Signature { get; init; }
    public required LoginPayload LoginPayload { get; init; }
}

public record LoginResponse
{
    public required string Authorizer { get; init; }
    public required string OwnerAuthorizer { get; init; }
    public required string Token { get; init; }
}

public record Nonce
{
    public required long LowerBound { get; init; }
    public required long UpperBound { get; init; }

    public static Nonce Empty => new Nonce
    {
        UpperBound = 0,
        LowerBound = 0
    };
}
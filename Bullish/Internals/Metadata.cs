namespace Bullish.Internals;

internal record Metadata
{
    public required string UserId { get; init; }
    public required string PublicKey { get; init; }
    public required string CredentialId { get; init; }

    public static Metadata Empty => new Metadata
    {
        UserId = string.Empty,
        PublicKey = string.Empty,
        CredentialId = string.Empty,
    };
}
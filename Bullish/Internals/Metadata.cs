namespace Bullish.Internals;

internal sealed record Metadata(string UserId, string PublicKey, string CredentialId)
{
    public static Metadata Empty => new(string.Empty, string.Empty, string.Empty);
}
namespace Bullish;

internal sealed record BxMetadata(string UserId, string PublicKey, string CredentialId)
{
    public static BxMetadata Empty => new(string.Empty, string.Empty, string.Empty);
}
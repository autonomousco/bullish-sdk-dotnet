using Org.BouncyCastle.Crypto.Parameters;

namespace Bullish.Signer;

public record EosPrivateKey(ECPrivateKeyParameters PrivateKey);

public record EosPublicKey(string EncodedPublicKey, ECPublicKeyParameters PublicKey);

using System.Security.Cryptography;
using System.Text;

namespace Bullish.Signer;

/// <summary>
/// Used to sign API requests to the Bullish exchange
/// </summary>
public static class RequestSigner
{
    /// <summary>
    /// Sign an API request for the Bullish exchange
    /// </summary>
    /// <param name="eosWifPrivateKey">API public key with format "PUB_R1_6zN...kYk"</param>
    /// <param name="eosWifPublicKey">API private key with format "PVT_R1_6zN...kYk"</param>
    /// <param name="jsonPayload">The JSON payload to be signed</param>
    /// <returns>A signature with format "SIG_R1_Kac...9jx"</returns>
    public static string Sign(string eosWifPrivateKey, string eosWifPublicKey, string jsonPayload)
    {
        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(jsonPayload));

        var privateKey = RequestSignerImpl.GetEosPrivateKey(eosWifPrivateKey);
        var publicKey = RequestSignerImpl.GetEosPublicKey(eosWifPublicKey);

        return RequestSignerImpl.SignRequest(digest, privateKey, publicKey);
    }
}
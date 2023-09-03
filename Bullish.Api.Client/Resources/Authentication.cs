using Bullish.Api.Client.BxClient;
using Bullish.Signer;

namespace Bullish.Api.Client.Resources;

public static class Authentication
{
    /// <summary>
    /// The API uses bearer based authentication. A JWT token is valid for 24 hours only.
    /// Sessions are currently tracked per user, and each user is allowed to create a maximum of 500 concurrent sessions.
    /// If a user exceeds this limit, an error (MAX_SESSION_COUNT_REACHED) is returned on the subsequent login requests.
    /// In order to free up unused sessions, users must logout.
    /// </summary>
    /// <param name="publicKey">Bullish account public key</param>
    /// <param name="privateKey">Bullish account private key</param>
    /// <param name="userId">An API key additionally has a metadata string assoicated with it which is displayed along side the key. You must base64 decode the metadata to extract your userId.</param>
    public static async Task<BxHttpResponse<LoginResponse>> Login(this BxHttpClient httpClient, string publicKey, string privateKey, string userId)
    {
        var utcNow = DateTimeOffset.UtcNow;
        var nonce = utcNow.ToUnixTimeSeconds();
        var expirationTime = nonce + 300;

        var loginPayload = new LoginPayload
        {
            UserId = userId,
            Nonce = nonce,
            ExpirationTime = expirationTime,
            BiometricsUsed = false,
            SessionKey = null,
        };

        var payloadJson = Extensions.Serialize(loginPayload);

        var signature = RequestSigner.Sign(privateKey, publicKey, payloadJson);
        
        var login = new Login
        {
            PublicKey = publicKey,
            LoginPayload = loginPayload,
            Signature = signature,
        };
        
        var bxPath = new BxPathBuilder(BxApiEndpoint.Login)
            .Build();

        return await httpClient.Post<LoginResponse, Login>(bxPath, login);
    }

    /// <summary>
    /// Users can better manage their sessions by logging out of unused sessions. 
    /// </summary>
    public static async Task<BxHttpResponse<LogoutResponse>> Logout(this BxHttpClient httpClient)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.Logout)
            .Build();

        return await httpClient.Get<LogoutResponse>(bxPath);  
    }
}
using System.Net.Http.Headers;
using Bullish.Signer;

namespace Bullish.Api.Client;

public class BxHttpClient
{
    private readonly BxMetadata _bxMetadata;
    private readonly EosPublicKey _publicKey;
    private readonly EosPrivateKey _privateKey;

    private string _authorizer = string.Empty;
    private string _jtw = string.Empty;
    private Nonce _nonce = Nonce.Empty;

    public BxHttpClient(string publicKey, string privateKey, string metadata)
    {
        _bxMetadata = Utilities.DeserializeBase64<BxMetadata>(metadata) ?? throw new ArgumentException("Invalid metadata");

        _privateKey = RequestSigner.GetEosPrivateKey(privateKey);
        _publicKey = RequestSigner.GetEosPublicKey(publicKey);
    }

    public async Task GetNonce()
    {
        var baseUrl = "https://api.exchange.bullish.com/trading-api/v1";
        var httpClient = new HttpClient();

        var response = await httpClient.GetAsync($"{baseUrl}/nonce");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            
            var nonce = Utilities.Deserialize<Nonce>(result);

            _nonce = nonce ?? throw new Exception("The nonce did not contain the expected data");
        }
    }

    public async Task Login()
    {
        var utcNow = DateTimeOffset.UtcNow;
        var nonce = utcNow.ToUnixTimeSeconds();
        var expirationTime = nonce + 300;

        var loginPayload = new LoginPayload
        {
            UserId = _bxMetadata.UserId,
            Nonce = nonce,
            ExpirationTime = expirationTime,
            BiometricsUsed = false,
            SessionKey = null,
        };

        var payloadJson = Utilities.Serialize(loginPayload);

        var signature = RequestSigner.Sign(_privateKey, _publicKey, payloadJson);

        var baseUrl = "https://api.exchange.bullish.com/trading-api/v2";
        var httpClient = new HttpClient();

        var login = new Login
        {
            PublicKey = _publicKey.EncodedPublicKey,
            LoginPayload = loginPayload,
            Signature = signature,
        };

        var authRequest = Utilities.Serialize(login);

        var response = await httpClient.PostAsync($"{baseUrl}/users/login", new StringContent(authRequest, new MediaTypeHeaderValue("application/json")));

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var loginResponse = Utilities.Deserialize<LoginResponse>(result);

            if (loginResponse is null)
                throw new Exception("The login response did not contain the expected data");

            _jtw = loginResponse.Token;
            _authorizer = loginResponse.Authorizer;
        }
    }
}
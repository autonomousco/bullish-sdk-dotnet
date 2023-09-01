using System.Net.Http.Headers;
using Bullish.Signer;

namespace Bullish.Api.Client.HttpClient;

public class BxHttpClient
{
    private readonly BxMetadata _bxMetadata;
    private readonly EosPublicKey _publicKey;
    private readonly EosPrivateKey _privateKey;

    private string _apiServer;
    private BxNonce _bxNonce = BxNonce.Empty;
    private string _authorizer = string.Empty;
    
    private string _jwt = string.Empty;
    private DateTime _jwtCreated = DateTime.MinValue;

    public BxHttpClient(string publicKey, string privateKey, string metadata)
    {
        _bxMetadata = Extensions.DeserializeBase64<BxMetadata>(metadata) ?? throw new ArgumentException("Invalid metadata");

        _privateKey = RequestSigner.GetEosPrivateKey(privateKey);
        _publicKey = RequestSigner.GetEosPublicKey(publicKey);

        // Set the default API server to Production
        _apiServer = Data.BxApiServers[BxApiServer.Production];
    }

    public BxHttpClient ConfigureApiServer(BxApiServer apiServer)
    {
        _apiServer = Data.BxApiServers[apiServer];
        return this;
    }

    public async Task<BxHttpResponse<T>> MakeRequest<T>(string path)
    {
        var url = $"{_apiServer}{path}";

        var httpClient = new System.Net.Http.HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);
        var response = await httpClient.GetAsync(url);

        var json = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var obj = Extensions.Deserialize<T>(json);
            
            return obj is not null ?
                BxHttpResponse<T>.Success(obj) : 
                BxHttpResponse<T>.Failure("Could not deserialize response.");
        }
        
        var bxHttpError = Extensions.Deserialize<BxHttpError>(json) ?? BxHttpError.Error("Unknown error");

        return BxHttpResponse<T>.Failure(bxHttpError);
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

        var payloadJson = Extensions.Serialize(loginPayload);

        var signature = RequestSigner.Sign(_privateKey, _publicKey, payloadJson);

        var baseUrl = "https://api.exchange.bullish.com/trading-api/v2";
        var httpClient = new System.Net.Http.HttpClient();

        var login = new Login
        {
            PublicKey = _publicKey.EncodedPublicKey,
            LoginPayload = loginPayload,
            Signature = signature,
        };

        var authRequest = Extensions.Serialize(login);

        var response = await httpClient.PostAsync($"{baseUrl}/users/login", new StringContent(authRequest, new MediaTypeHeaderValue("application/json")));

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var loginResponse = Extensions.Deserialize<LoginResponse>(result);

            if (loginResponse is null)
                throw new Exception("The login response did not contain the expected data");

            _jwt = loginResponse.Token;
            _jwtCreated = DateTime.UtcNow;
            _authorizer = loginResponse.Authorizer;
        }
    }
}
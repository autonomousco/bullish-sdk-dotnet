using System.Net.Http.Headers;
using Bullish.Signer;

namespace Bullish.Api.Client;

public class BxHttpClient
{
    private readonly BxMetadata _bxMetadata;
    private readonly EosPublicKey _publicKey;
    private readonly EosPrivateKey _privateKey;

    private string _authorizer = string.Empty;
    private string _jwt = string.Empty;
    private DateTime _jwtCreated = DateTime.MinValue;
    
    private Nonce _nonce = Nonce.Empty;

    private string _apiServer;

    public BxHttpClient(string publicKey, string privateKey, string metadata)
    {
        _bxMetadata = Utilities.DeserializeBase64<BxMetadata>(metadata) ?? throw new ArgumentException("Invalid metadata");

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

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);
        var response = await httpClient.GetAsync(url);

        var json = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var obj = Utilities.Deserialize<T>(json);
            
            return obj is not null ?
                BxHttpResponse<T>.Success(obj) : 
                BxHttpResponse<T>.Failure("Could not deserialize response.");
        }
        
        var bxHttpError = Utilities.Deserialize<BxHttpError>(json) ?? BxHttpError.Error("Unknown error");

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

            _jwt = loginResponse.Token;
            _jwtCreated = DateTime.UtcNow;
            _authorizer = loginResponse.Authorizer;
        }
    }

    private async Task GetNonce()
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

    private bool IsNonceValid()
    {
        // TODO: Shift into Nonce record
        if (_nonce.LowerBound == 0 || _nonce.UpperBound == 0)
            return false;

        var todayUtc = DateTime.UtcNow.TodayUtc();

        var localLower = todayUtc.ToUnixTimeMicroseconds();
        var localUpper = localLower + 86399999000; // Add time up to 1ms before midnight

        return _nonce.LowerBound == localLower && _nonce.UpperBound == localUpper;
    }
}
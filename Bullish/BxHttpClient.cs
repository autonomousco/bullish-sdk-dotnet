using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using Bullish.Internals;
using Bullish.Schemas;
using Bullish.Signer;

namespace Bullish;

public sealed class BxHttpClient
{
    private readonly bool _autoLogin;
    private readonly string _apiServer;

    private string _publicKey = string.Empty;
    private string _privateKey = string.Empty;
    private Metadata _metadata = Metadata.Empty;

    private Nonce _nonce = Nonce.Empty;
    private AuthToken _authToken = AuthToken.Empty;
    private string _authorizer = string.Empty;

    private Dictionary<string, Market> _markets = new();

    public BxHttpClient(string publicKey = "", string privateKey = "", string metadata = "", BxApiServer apiServer = BxApiServer.Production, bool autoLogin = false)
    {
        _apiServer = Constants.BxApiServers[apiServer];
        _autoLogin = autoLogin;

        var emptyCredentials = string.IsNullOrWhiteSpace(publicKey) || string.IsNullOrWhiteSpace(privateKey) || string.IsNullOrWhiteSpace(metadata);

        if (_autoLogin && emptyCredentials)
            throw new Exception("Public key, private key and metadata must be set if auto-login is enabled.");

        if (!emptyCredentials)
        {
            _privateKey = privateKey;
            _publicKey = publicKey;
            _metadata = Extensions.DeserializeBase64<Metadata>(metadata) ?? throw new ArgumentException("Invalid metadata");
        }
    }

    internal async Task<decimal> FormatValue(PrecisionType type, string symbol, decimal value)
    {
        if (!_markets.ContainsKey(symbol))
            await UpdateMarkets();

        if (!_markets.TryGetValue(symbol, out var market))
            throw new Exception($"Cannot find market for symbol {symbol}");

        var precision = value.GetDecimalPlaces();
        var desiredPrecision = type switch
        {
            PrecisionType.BasePrecision => market.BasePrecision,
            PrecisionType.QuotePrecision => market.QuotePrecision,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        if (precision > desiredPrecision)
            throw new Exception($"Value {value} has {type} of {precision}. Must be {desiredPrecision} or less to avoid overflow.");

        return decimal.Parse(value.ToString($"F{desiredPrecision}"));
    }

    /// <summary>
    /// The API uses bearer based authentication. A JWT token is valid for 24 hours only.
    /// Sessions are currently tracked per user, and each user is allowed to create a maximum of 500 concurrent sessions.
    /// If a user exceeds this limit, an error (MAX_SESSION_COUNT_REACHED) is returned on the subsequent login requests.
    /// In order to free up unused sessions, users must logout.
    /// </summary>
    /// <param name="storeResult">Store the resulting JWT and Nonce in the BxHttpClient instance?</param>
    public async Task<BxHttpResponse<Login>> Login(bool storeResult = true)
    {
        await UpdateNonce();

        var nonce = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var loginPayload = new
        {
            UserId = _metadata.UserId,
            Nonce = nonce,
            ExpirationTime = nonce + 300,
            BiometricsUsed = false,
            SessionKey = (string?)null,
        };

        var payloadJson = Extensions.Serialize(loginPayload);

        var signature = RequestSigner.Sign(_privateKey, _publicKey, payloadJson);

        var login = new
        {
            PublicKey = _publicKey,
            LoginPayload = loginPayload,
            Signature = signature,
        };

        var bxPath = new EndpointPathBuilder(BxApiEndpoint.Login)
            .Build();

        var url = $"{_apiServer}{bxPath.Path}";

        var httpClient = new HttpClient();

        var bodyJson = Extensions.Serialize(login);

        var response = await httpClient.PostAsync(url, new StringContent(bodyJson, new MediaTypeHeaderValue("application/json")));

        var loginResponse = await ProcessResponse<Login>(response);

        if (loginResponse.IsSuccess && storeResult)
        {
            _authToken = new AuthToken(loginResponse.Result!.Token);
            _authorizer = loginResponse.Result.Authorizer;
            // _ownerAuthorizer = loginResponse.Result.OwnerAuthorizer;
        }

        return loginResponse;
    }

    /// <summary>
    /// Users can better manage their sessions by logging out of unused sessions. 
    /// </summary>
    public async Task<BxHttpResponse<Empty>> Logout()
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.Logout)
            .Build();

        var logoutResponse = await Get<Empty>(bxPath);

        if (logoutResponse.IsSuccess)
        {
            _nonce = Nonce.Empty;
            _authToken = AuthToken.Empty;
            _authorizer = string.Empty;
            // _ownerAuthorizer = string.Empty;
        }

        return logoutResponse;
    }

    internal async Task<BxHttpResponse<TResult>> Post<TResult, TCommand>(EndpointPath path, TCommand command) 
        where TCommand : Command 
        where TResult : notnull, new()
    {
        var url = $"{_apiServer}{path.Path}";

        var httpClient = new HttpClient();

        if (!_authToken.IsValid)
        {
            if (_autoLogin)
                await Login();
            else
                throw new Exception("No valid JWT was found. Please login.");
        }

        // Authorize the command with data available only after login
        command = command with
        {
            Timestamp = $"{DateTime.UtcNow.ToUnixTimeMilliseconds()}",
            Nonce = $"{_nonce.NextValue()}",
            Authorizer = _authorizer,
        };

        var bodyJson = Extensions.Serialize(command);

        var signature = RequestSigner.Sign(_privateKey, _publicKey, bodyJson);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken.Jwt);
        httpClient.DefaultRequestHeaders.Add("BX-SIGNATURE", signature);
        httpClient.DefaultRequestHeaders.Add("BX-TIMESTAMP", command.Timestamp);
        httpClient.DefaultRequestHeaders.Add("BX-NONCE", command.Nonce);
        httpClient.DefaultRequestHeaders.Add("BX-NONCE-WINDOW-ENABLED", "false");

        var response = await httpClient.PostAsync(url, new StringContent(bodyJson, new MediaTypeHeaderValue("application/json")));

        return await ProcessResponse<TResult>(response);
    }

    internal async Task<BxHttpResponse<TResult>> Get<TResult>(EndpointPath path) where TResult : notnull, new()
    {
        var url = $"{_apiServer}{path.Path}";

        var httpClient = new HttpClient();

        if (path.UseAuth)
        {
            if (!_authToken.IsValid)
            {
                if (_autoLogin)
                    await Login();
                else
                    throw new Exception("No valid JWT was found. Please login.");
            }

            if (!_nonce.IsValid())
                throw new Exception("No valid Nonce was found. Please login.");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken.Jwt);
        }

        var response = await httpClient.GetAsync(url);

        return await ProcessResponse<TResult>(response, path.UsePagination);
    }

    private static async Task<BxHttpResponse<TResult>> ProcessResponse<TResult>(HttpResponseMessage response, bool usePagination = false) where TResult : notnull, new()
    {
        var json = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var pageLinks = BxPageLinks.Empty;

            if (usePagination)
            {
                var jsonNode = JsonNode.Parse(json);

                json = jsonNode?["data"]?.ToJsonString();

                if (string.IsNullOrWhiteSpace(json))
                    return BxHttpResponse<TResult>.Failure("Failed to get data from paginated result.");

                var linksJson = jsonNode?["links"]?.ToJsonString();

                if (string.IsNullOrWhiteSpace(linksJson))
                    return BxHttpResponse<TResult>.Failure("Failed to deserialize page links.");

                if (Extensions.Deserialize<BxPageLinks>(linksJson) is { } links)
                    pageLinks = links;
            }

            // If there's no json payload in the response (i.e. logout), just return success
            if (string.IsNullOrWhiteSpace(json))
                return BxHttpResponse<TResult>.Success();

            var obj = Extensions.Deserialize<TResult>(json);

            return obj is not null ? BxHttpResponse<TResult>.Success(obj, pageLinks) : BxHttpResponse<TResult>.Failure("Could not deserialize response.");
        }

        // The response did not return a success StatusCode, try to parse the error
        try
        {
            var bxHttpError = Extensions.Deserialize<BxHttpError>(json) ?? BxHttpError.Error("Unknown error");
            return BxHttpResponse<TResult>.Failure(bxHttpError);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BxHttpResponse<TResult>.Failure(BxHttpError.Error(response.StatusCode, json));
        }
    }

    private async Task UpdateMarkets()
    {
        var marketsResponse = await this.GetMarkets();

        if (!marketsResponse.IsSuccess)
            throw new Exception($"Failed to get Markets. Reason:{marketsResponse.Error.Message}");

        _markets = marketsResponse.Result.ToDictionary(key => key.Symbol, value => value);
    }

    private async Task UpdateNonce()
    {
        // Configure the nonce
        var nonceResponse = await this.GetNonce();

        if (!nonceResponse.IsSuccess)
            throw new Exception($"Failed to get Nonce. Reason:{nonceResponse.Error.Message}");

        _nonce = nonceResponse.Result;
    }
}
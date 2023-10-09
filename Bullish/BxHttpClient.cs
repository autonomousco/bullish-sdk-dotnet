using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Bullish.Internals;
using Bullish.Schemas;
using Bullish.Signer;

namespace Bullish;

public sealed class BxHttpClient
{
    // TODO: Wrap into a record with validation
    private readonly bool _autoLogin;
    private readonly string _apiServer;
    private readonly AuthMode _authMode;
    private readonly string _publicKey = string.Empty;
    private readonly string _privateKey = string.Empty;
    private readonly Metadata _metadata = Metadata.Empty;

    private Nonce _nonce = Nonce.Empty;
    private AuthToken _authToken = AuthToken.Empty;
    private string _authorizer = string.Empty;

    private Dictionary<string, Market> _markets = new();

    public BxHttpClient(BxApiServer apiServer = BxApiServer.Production)
    {
        _apiServer = Constants.BxApiServers[apiServer];
    }

    public BxHttpClient(string publicKey, string privateKey, string metadata = "", BxApiServer apiServer = BxApiServer.Production, bool autoLogin = false)
    {
        _publicKey = !string.IsNullOrWhiteSpace(publicKey) ? publicKey : throw new Exception("Public key cannot be empty.");
        _privateKey = !string.IsNullOrWhiteSpace(privateKey) ? privateKey : throw new Exception("Private key cannot be empty.");

        _authMode = publicKey[..4].ToUpperInvariant() switch
        {
            "HMAC" => AuthMode.Hmac,
            "PUB_" => AuthMode.Eos,
            _ => throw new ArgumentException("Public key type is not supported.")
        };

        if (_authMode == AuthMode.Eos)
        {
            if (string.IsNullOrWhiteSpace(metadata))
                throw new Exception("Metadata cannot be empty when using EOS authentication.");

            _metadata = Extensions.DeserializeBase64<Metadata>(metadata) ?? throw new ArgumentException("Invalid metadata");
        }

        _apiServer = Constants.BxApiServers[apiServer];
        _autoLogin = autoLogin;
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

    private async Task<BxHttpResponse<Login>> LoginHmac()
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.LoginHmac)
            .Build();

        var url = $"{_apiServer}{bxPath.Path}";

        var httpClient = new HttpClient();

        var nonce = $"{_nonce.NextValue()}";
        var timestamp = $"{DateTime.UtcNow.ToUnixTimeMilliseconds()}";

        var message = $"{timestamp}{nonce}GET/trading-api/v1/users/hmac/login";

        using var hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(_privateKey));

        var digest = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(message));
        var signature = Convert.ToHexString(digest).ToLowerInvariant();

        httpClient.DefaultRequestHeaders.Add("BX-PUBLIC-KEY", _publicKey);
        httpClient.DefaultRequestHeaders.Add("BX-NONCE", nonce);
        httpClient.DefaultRequestHeaders.Add("BX-SIGNATURE", signature);
        httpClient.DefaultRequestHeaders.Add("BX-TIMESTAMP", timestamp);

        var response = await httpClient.GetAsync(url);

        return await ProcessResponse<Login>(response);
    }

    private async Task<BxHttpResponse<Login>> LoginEos()
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.Login)
            .Build();

        var url = $"{_apiServer}{bxPath.Path}";

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

        var httpClient = new HttpClient();

        var bodyJson = Extensions.Serialize(login);

        var response = await httpClient.PostAsync(url, new StringContent(bodyJson, new MediaTypeHeaderValue("application/json")));

        return await ProcessResponse<Login>(response);
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

        var loginResponse = _authMode switch
        {
            AuthMode.Eos => await LoginEos(),
            AuthMode.Hmac => await LoginHmac(),
            _ => throw new ArgumentException("Invalid authentication mode")
        };

        if (loginResponse.IsSuccess && storeResult)
        {
            _authToken = new AuthToken(loginResponse.Result.Token);
            _authorizer = loginResponse.Result.Authorizer;
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
        where TCommand : ICommand
        where TResult : notnull, new()
    {
        #region TODO: Extract common code

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
        var commandBody = new
        {
            Timestamp = $"{DateTime.UtcNow.ToUnixTimeMilliseconds()}",
            Nonce = $"{_nonce.NextValue()}",
            Authorizer = _authorizer,
            Command = command,
        };

        var bodyJson = Extensions.Serialize(commandBody);

        var signature = Sign(bodyJson, _privateKey, _publicKey, _authMode);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken.Jwt);
        httpClient.DefaultRequestHeaders.Add("BX-SIGNATURE", signature);
        httpClient.DefaultRequestHeaders.Add("BX-TIMESTAMP", commandBody.Timestamp);
        httpClient.DefaultRequestHeaders.Add("BX-NONCE", commandBody.Nonce);
        httpClient.DefaultRequestHeaders.Add("BX-NONCE-WINDOW-ENABLED", "false");

        #endregion

        var response = await httpClient.PostAsync(url, new StringContent(bodyJson, new MediaTypeHeaderValue("application/json")));

        return await ProcessResponse<TResult>(response);
    }

    internal async Task<BxHttpResponse<TResult>> Delete<TResult, TCommand>(EndpointPath path, TCommand command)
        where TCommand : ICommand
        where TResult : notnull, new()
    {
        #region TODO: Extract common code

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
        var commandBody = new
        {
            Timestamp = $"{DateTime.UtcNow.ToUnixTimeMilliseconds()}",
            Nonce = $"{_nonce.NextValue()}",
            Authorizer = _authorizer,
            Command = command,
        };

        var bodyJson = Extensions.Serialize(commandBody);

        var signature = Sign(bodyJson, _privateKey, _publicKey, _authMode);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken.Jwt);
        httpClient.DefaultRequestHeaders.Add("BX-SIGNATURE", signature);
        httpClient.DefaultRequestHeaders.Add("BX-TIMESTAMP", commandBody.Timestamp);
        httpClient.DefaultRequestHeaders.Add("BX-NONCE", commandBody.Nonce);
        httpClient.DefaultRequestHeaders.Add("BX-NONCE-WINDOW-ENABLED", "false");

        #endregion

        var response = await httpClient.DeleteAsync(url);

        return await ProcessResponse<TResult>(response, path.UsePagination);
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

    private static string Sign(string message, string privateKey, string publicKey, AuthMode authMode)
    {
        switch (authMode)
        {
            case AuthMode.Hmac:
            {
                var shaDigest = SHA256.HashData(Encoding.UTF8.GetBytes(message));
                var hexSha = Convert.ToHexString(shaDigest).ToLowerInvariant();

                using var hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(privateKey));
                var hmacDigest = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(hexSha));

                return Convert.ToHexString(hmacDigest).ToLowerInvariant();
            }
            case AuthMode.Eos:
                return RequestSigner.Sign(privateKey, publicKey, message);
            default:
                throw new Exception("Invalid AuthMode for signing.");
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

        if (!nonceResponse.Result.IsValid())
            throw new Exception("Nonce from server was invalid.");

        _nonce = nonceResponse.Result;
    }
}
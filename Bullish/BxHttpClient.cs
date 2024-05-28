using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Bullish.Internals;
using Bullish.Schemas;

[assembly: InternalsVisibleTo("Bullish.Tests")]

namespace Bullish;

public sealed class BxHttpClient
{
    // TODO: Wrap into a record with validation
    private readonly bool _autoLogin;
    private readonly string _apiServer;
    private readonly AuthMode _authMode;
    private readonly string _publicKey = string.Empty;
    private readonly string _privateKey = string.Empty;
    private readonly Metadata _metaData = Metadata.Empty;

    private AuthToken _authToken = AuthToken.Empty;
    private string _authorizer = string.Empty;

    private Dictionary<string, Market> _markets = new();

    public BxHttpClient(BxApiServer apiServer = BxApiServer.Production)
    {
        _apiServer = Constants.BxApiServers[apiServer];
    }

    public BxHttpClient(string publicKey, string privateKey, string metaData = "", AuthMode authMode = AuthMode.Hmac, BxApiServer apiServer = BxApiServer.Production, bool autoLogin = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(publicKey, nameof(publicKey));
        ArgumentException.ThrowIfNullOrWhiteSpace(privateKey, nameof(privateKey));

        _publicKey = publicKey;
        _privateKey = privateKey;
        _apiServer = Constants.BxApiServers[apiServer];
        _autoLogin = autoLogin;
        _authMode = authMode;

        switch (authMode)
        {
            case AuthMode.Hmac:
            {
                if (!publicKey.StartsWith("HMAC"))
                    throw new Exception("Invalid HMAC Public Key");
                break;
            }
            case AuthMode.Ecdsa:
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(metaData, nameof(metaData));

                var metaDataJson = Encoding.UTF8.GetString(Convert.FromBase64String(metaData));

                _metaData = Extensions.Deserialize<Metadata>(metaDataJson) ??
                            throw new ArgumentException("Metadata is invalid", nameof(metaData));

                // TODO: Wrap the ECDSA & HMAC Auth and Signer in a class
                using var publicKeyEcdsa = ECDsa.Create();
                publicKeyEcdsa.ImportFromPem(publicKey);

                using var privateKeyEcdsa = ECDsa.Create();
                privateKeyEcdsa.ImportFromPem(privateKey);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(authMode), authMode, "Invalid Authentication Mode");
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

    private async Task<BxHttpResponse<Login>> LoginHmac()
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.LoginHmac)
            .Build();

        var url = $"{_apiServer}{bxPath.Path}";

        var httpClient = new HttpClient();

        var nonce = $"{DateTime.UtcNow.ToUnixTimeMicroseconds()}";
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

    private async Task<BxHttpResponse<Login>> LoginEcdsa()
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.LoginEcdsa)
            .Build();

        var url = $"{_apiServer}{bxPath.Path}";

        var httpClient = new HttpClient();

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var expirationTime = timestamp + 300;

        var loginPayload = new
        {
            UserId = _metaData.UserId,
            Nonce = timestamp,
            ExpirationTime = expirationTime,
            BiometricsUsed = false,
            SessionKey = (string)null!,
        };

        var loginPayloadJson = Extensions.Serialize(loginPayload);

        var ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(_privateKey);

        var signature = ecdsa.SignData(Encoding.UTF8.GetBytes(loginPayloadJson), HashAlgorithmName.SHA256, DSASignatureFormat.Rfc3279DerSequence);
        var signatureBase64 = Convert.ToBase64String(signature);

        var body = new
        {
            publicKey = _publicKey,
            signature = signatureBase64,
            loginPayload,
        };

        var bodyJson = Extensions.Serialize(body);

        var response = await httpClient.PostAsync(url, new StringContent(bodyJson, new MediaTypeHeaderValue("application/json")));

        return await ProcessResponse<Login>(response);
    }

    /// <summary>
    /// The API uses bearer based authentication. A JWT token is valid for 24 hours only.
    /// Sessions are currently tracked per user, and each user is allowed to create a maximum of 500 concurrent sessions.
    /// If a user exceeds this limit, an error (MAX_SESSION_COUNT_REACHED) is returned on the subsequent login requests.
    /// In order to free up unused sessions, users must logout.
    /// </summary>
    /// <param name="storeResult">Store the resulting JWT in the BxHttpClient instance</param>
    public async Task<BxHttpResponse<Login>> Login(bool storeResult = true)
    {
        // If we have an existing session, make sure it is logged out
        if (!_authToken.IsEmpty)
            await Logout();

        var loginResponse = _authMode switch
        {
            AuthMode.Hmac => await LoginHmac(),
            AuthMode.Ecdsa => await LoginEcdsa(),
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

        // Logout, but make sure we don't autologin again
        var logoutResponse = await Get<Empty>(bxPath, ignoreAutoLogin: true);

        if (logoutResponse.IsSuccess)
        {
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

        var timestamp = $"{DateTime.UtcNow.ToUnixTimeMilliseconds()}";
        var nonce = $"{DateTime.UtcNow.ToUnixTimeMicroseconds()}";

        var bodyJson = Extensions.Serialize(command);

        var signature = path.Path.Contains("/v2/") ? SignV2(timestamp, nonce, "POST", $"/trading-api{path.Path}", bodyJson, _privateKey, _authMode) : Sign(bodyJson, _privateKey, _authMode);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken.Jwt);
        httpClient.DefaultRequestHeaders.Add("BX-SIGNATURE", signature);
        httpClient.DefaultRequestHeaders.Add("BX-TIMESTAMP", timestamp);
        httpClient.DefaultRequestHeaders.Add("BX-NONCE", nonce);
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
            Nonce = $"{DateTime.UtcNow.ToUnixTimeMicroseconds()}",
            Authorizer = _authorizer,
            Command = command,
        };

        var bodyJson = Extensions.Serialize(commandBody);

        var signature = Sign(bodyJson, _privateKey, _authMode);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken.Jwt);
        httpClient.DefaultRequestHeaders.Add("BX-SIGNATURE", signature);
        httpClient.DefaultRequestHeaders.Add("BX-TIMESTAMP", commandBody.Timestamp);
        httpClient.DefaultRequestHeaders.Add("BX-NONCE", commandBody.Nonce);
        httpClient.DefaultRequestHeaders.Add("BX-NONCE-WINDOW-ENABLED", "false");

        #endregion

        var response = await httpClient.DeleteAsync(url);

        return await ProcessResponse<TResult>(response, path.UsePagination);
    }

    internal async Task<BxHttpResponse<TResult>> Get<TResult>(EndpointPath path, bool ignoreAutoLogin = false) where TResult : notnull, new()
    {
        var url = $"{_apiServer}{path.Path}";

        var httpClient = new HttpClient();

        if (path.UseAuth && !ignoreAutoLogin)
        {
            if (!_authToken.IsValid)
            {
                if (_autoLogin)
                    await Login();
                else
                    throw new Exception("No valid JWT was found. Please login.");
            }

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
            if (string.IsNullOrWhiteSpace(json))
                return BxHttpResponse<TResult>.Failure(BxHttpError.Error(response.StatusCode, response.ReasonPhrase ?? "Unknown"));
            
            var bxHttpError = Extensions.Deserialize<BxHttpError>(json) ?? BxHttpError.Error("Unknown error");
                return BxHttpResponse<TResult>.Failure(bxHttpError);
            
        }
        catch (Exception ex)
        {
            return BxHttpResponse<TResult>.Failure(BxHttpError.Error(response.StatusCode, $"Error:{ex.Message}, JSON:{json}"));
        }
    }

    private static string Sign(string message, string privateKey, AuthMode authMode)
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
            default:
                throw new Exception("Invalid AuthMode for signing.");
        }
    }

    private static string SignV2(string timestamp, string nonce, string method, string path, string bodyJson, string privateKey, AuthMode authMode)
    {
        var message = $"{timestamp}{nonce}{method}{path}{bodyJson}";

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
            case AuthMode.Ecdsa:
            {
                var ecdsa = ECDsa.Create();
                ecdsa.ImportFromPem(privateKey);

                var signature = ecdsa.SignData(Encoding.UTF8.GetBytes(message), HashAlgorithmName.SHA256, DSASignatureFormat.Rfc3279DerSequence);
                return Convert.ToBase64String(signature);
            }
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
}
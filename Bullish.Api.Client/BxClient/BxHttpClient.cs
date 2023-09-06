using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using Bullish.Api.Client.Resources;
using Bullish.Signer;

namespace Bullish.Api.Client.BxClient;

public class BxHttpClient
{
    private readonly string _publicKey;
    private readonly string _privateKey;
    private readonly BxMetadata _bxMetadata;

    private string _apiServer;

    private BxNonce _bxNonce = BxNonce.Empty;
    private BxAuthToken _bxAuthToken = BxAuthToken.Empty;
    private string _authorizer = string.Empty;
    private string _ownerAuthorizer = string.Empty;

    public CommandRequest GetCommandRequest() => new()
    {
        Timestamp = DateTime.UtcNow.ToUnixTimeMilliseconds().ToString(),
        Nonce = $"{_bxNonce.NextValue()}",
        Authorizer = _authorizer,
    };

    public BxHttpClient(string publicKey, string privateKey, string metadata)
    {
        _bxMetadata = Extensions.DeserializeBase64<BxMetadata>(metadata) ?? throw new ArgumentException("Invalid metadata");

        _privateKey = privateKey;
        _publicKey = publicKey;

        // Set the default API server to Production
        _apiServer = Data.BxApiServers[BxApiServer.Production];
    }

    public BxHttpClient ConfigureApi(BxApiServer apiServer)
    {
        _apiServer = Data.BxApiServers[apiServer];
        return this;
    }

    /// <summary>
    /// The API uses bearer based authentication. A JWT token is valid for 24 hours only.
    /// Sessions are currently tracked per user, and each user is allowed to create a maximum of 500 concurrent sessions.
    /// If a user exceeds this limit, an error (MAX_SESSION_COUNT_REACHED) is returned on the subsequent login requests.
    /// In order to free up unused sessions, users must logout.
    /// </summary>
    /// <param name="storeResult">Store the resulting JWT and Nonce in the BxHttpClient instance?</param>
    public async Task<BxHttpResponse<LoginResponse>> Login(bool storeResult = true)
    {
        await UpdateNonce();

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

        var login = new Login
        {
            PublicKey = _publicKey,
            LoginPayload = loginPayload,
            Signature = signature,
        };

        var bxPath = new BxPathBuilder(BxApiEndpoint.Login)
            .Build();

        var url = $"{_apiServer}{bxPath.Path}";

        var httpClient = new HttpClient();

        var bodyJson = Extensions.Serialize(login);

        var response = await httpClient.PostAsync(url, new StringContent(bodyJson, new MediaTypeHeaderValue("application/json")));

        var loginResponse = await ProcessResponse<LoginResponse>(response);

        if (loginResponse.IsSuccess && storeResult)
        {
            _bxAuthToken = BxAuthToken.New(loginResponse.Result!.Token);
            _authorizer = loginResponse.Result.Authorizer;
            _ownerAuthorizer = loginResponse.Result.OwnerAuthorizer;
        }

        return loginResponse;
    }

    /// <summary>
    /// Users can better manage their sessions by logging out of unused sessions. 
    /// </summary>
    public async Task<BxHttpResponse<LogoutResponse>> Logout()
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.Logout)
            .Build();

        var logoutResponse = await Get<LogoutResponse>(bxPath);

        if (logoutResponse.IsSuccess)
        {
            _bxNonce = BxNonce.Empty;
            _bxAuthToken = BxAuthToken.Empty;
            _authorizer = string.Empty;
            _ownerAuthorizer = string.Empty;
        }

        return logoutResponse;
    }

    public async Task<BxHttpResponse<TResult>> Post<TResult, TPayload>(BxPath path, TPayload payload)
    {
        var url = $"{_apiServer}{path.Path}";

        var httpClient = new HttpClient();

        if (payload is not ICommandRequest commandRequest)
            throw new Exception("Validated POST requests must use a CommandRequest object,");

        if (!_bxAuthToken.IsValid)
            throw new Exception("No valid JWT was found. Please login.");

        var bodyJson = Extensions.Serialize(payload);

        var signature = RequestSigner.Sign(_privateKey, _publicKey, bodyJson);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bxAuthToken.Jwt);
        httpClient.DefaultRequestHeaders.Add("BX-SIGNATURE", signature);
        httpClient.DefaultRequestHeaders.Add("BX-TIMESTAMP", commandRequest.Timestamp);
        httpClient.DefaultRequestHeaders.Add("BX-NONCE", commandRequest.Nonce);
        httpClient.DefaultRequestHeaders.Add("BX-NONCE-WINDOW-ENABLED", "false");

        var response = await httpClient.PostAsync(url, new StringContent(bodyJson, new MediaTypeHeaderValue("application/json")));

        return await ProcessResponse<TResult>(response);
    }

    public async Task<BxHttpResponse<TResult>> Get<TResult>(BxPath path)
    {
        var url = $"{_apiServer}{path.Path}";

        var httpClient = new HttpClient();

        if (path.UseAuth)
        {
            if (!_bxNonce.IsValid())
                throw new Exception("No valid Nonce was found. Please login.");

            if (!_bxAuthToken.IsValid)
                throw new Exception("No valid JWT was found. Please login.");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bxAuthToken.Jwt);
        }

        var response = await httpClient.GetAsync(url);

        return await ProcessResponse<TResult>(response, path.UsePagination);
    }

    private static async Task<BxHttpResponse<TResult>> ProcessResponse<TResult>(HttpResponseMessage response, bool usePagination = false)
    {
        var json = await response.Content.ReadAsStringAsync();

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

        if (response.IsSuccessStatusCode)
        {
            // If there's no json payload in the response (i.e. logout), just return success
            if (string.IsNullOrWhiteSpace(json))
                return BxHttpResponse<TResult>.Success();

            var obj = Extensions.Deserialize<TResult>(json);

            return obj is not null ? BxHttpResponse<TResult>.Success(obj, pageLinks) : BxHttpResponse<TResult>.Failure("Could not deserialize response.");
        }

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

    private async Task UpdateNonce()
    {
        // Configure the nonce
        var nonceResponse = await this.GetNonce();

        if (!nonceResponse.IsSuccess)
            throw new Exception($"Failed to get Nonce. Reason:{nonceResponse.Error.Message}");

        if (nonceResponse.Result is null)
            throw new Exception("Response did not contain a valid Nonce.");

        _bxNonce = nonceResponse.Result;
    }
}
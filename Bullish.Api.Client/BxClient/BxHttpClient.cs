using System.Net.Http.Headers;
using Bullish.Api.Client.Resources;

namespace Bullish.Api.Client.BxClient;

public class BxHttpClient
{
    private record EmptyPayload();
    
    private readonly BxMetadata _bxMetadata;
    private readonly string _publicKey;
    private readonly string _privateKey;

    private string _apiServer;

    // private BxNonce _bxNonce = BxNonce.Empty;
    private BxAuthToken _bxAuthToken = BxAuthToken.Empty;

    public BxHttpClient(string publicKey, string privateKey, string metadata)
    {
        _bxMetadata = Extensions.DeserializeBase64<BxMetadata>(metadata) ?? throw new ArgumentException("Invalid metadata");

        _privateKey = privateKey;
        _publicKey = publicKey;

        // Set the default API server to Production
        _apiServer = Data.BxApiServers[BxApiServer.Production];
    }

    public BxHttpClient ConfigureApiServer(BxApiServer apiServer)
    {
        _apiServer = Data.BxApiServers[apiServer];
        return this;
    }

    public Task<BxHttpResponse<TResult>> Post<TResult>(BxPath path) => Post<TResult, EmptyPayload>(path, new EmptyPayload());

    public async Task<BxHttpResponse<TResult>> Post<TResult, TPayload>(BxPath path, TPayload payload)  
    {
        var url = $"{_apiServer}{path.Path}";

        var httpClient = new HttpClient();

        var bodyJson = payload is EmptyPayload ? "{}" : Extensions.Serialize(payload);

        if (path.UseAuth)
        {
            if (!_bxAuthToken.IsValid)
            {
                var loginResponse = await this.Login(_publicKey, _privateKey, _bxMetadata.UserId);

                if (!loginResponse.IsSuccess)
                    return BxHttpResponse<TResult>.Failure(loginResponse.Error);

                if (loginResponse.Result is null)
                    return BxHttpResponse<TResult>.Failure("Response did not contain a valid JWT token");

                _bxAuthToken = BxAuthToken.New(loginResponse.Result.Token);
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bxAuthToken.Jwt);
        }

        var response = await httpClient.PostAsync(url, new StringContent(bodyJson, new MediaTypeHeaderValue("application/json")));

        return await ProcessResponse<TResult>(response);
    }

    public async Task<BxHttpResponse<TResult>> Get<TResult>(BxPath path)
    {
        var url = $"{_apiServer}{path.Path}";

        var httpClient = new HttpClient();

        if (path.UseAuth)
        {
            if (!_bxAuthToken.IsValid)
            {
                // If we call logout, but we are already logged out, then early out
                if (path.Endpoint == BxApiEndpoint.Logout)
                    return BxHttpResponse<TResult>.Success();
                
                var loginResponse = await this.Login(_publicKey, _privateKey, _bxMetadata.UserId);

                if (!loginResponse.IsSuccess)
                    return BxHttpResponse<TResult>.Failure(loginResponse.Error);

                if (loginResponse.Result is null)
                    return BxHttpResponse<TResult>.Failure("Response did not contain a valid JWT token");

                _bxAuthToken = BxAuthToken.New(loginResponse.Result.Token);
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bxAuthToken.Jwt);
        }

        var response = await httpClient.GetAsync(url);

        return await ProcessResponse<TResult>(response);
    }

    private static async Task<BxHttpResponse<TResult>> ProcessResponse<TResult>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            // If there's no json payload in the response (i.e. logout), just return success
            if (string.IsNullOrWhiteSpace(json))
                return BxHttpResponse<TResult>.Success();
            
            var obj = Extensions.Deserialize<TResult>(json);

            return obj is not null ? BxHttpResponse<TResult>.Success(obj) : BxHttpResponse<TResult>.Failure("Could not deserialize response.");
        }

        try
        {
            var bxHttpError = Extensions.Deserialize<BxHttpError>(json) ?? BxHttpError.Error("Unknown error");
            return BxHttpResponse<TResult>.Failure(bxHttpError);
        }
        catch (Exception ex)
        {
            return BxHttpResponse<TResult>.Failure(BxHttpError.Error(response.StatusCode, json));
        }
    }
}
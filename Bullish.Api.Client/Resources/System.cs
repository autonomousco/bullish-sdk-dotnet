using Bullish.Api.Client.HttpClient;

namespace Bullish.Api.Client.Resources;

internal static class System
{
    /// <summary>
    /// Get the current nonce range.
    /// The lower bound of nonce range is EPOCH start of day in microseconds,
    /// and upper bound of nonce range is EPOCH end of day in microseconds.
    /// </summary>
    public static async Task<BxHttpResponse<BxNonce>> GetNonce(this BxHttpClient httpClient)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.Nonce);
        return await httpClient.MakeRequest<BxNonce>(pathBuilder.Path);
    }
    
    /// <summary>
    /// Get the current Exchange Time
    /// </summary>
    public static async Task<BxHttpResponse<ExchangeTime>> GetTime(this BxHttpClient httpClient)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.Time);
        return await httpClient.MakeRequest<ExchangeTime>(pathBuilder.Path);
    }
}
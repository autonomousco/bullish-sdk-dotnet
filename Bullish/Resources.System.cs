using Bullish.Internals;
using Bullish.Schemas;

namespace Bullish;

internal static class ResourcesInternal
{
    /// <summary>
    /// Get the current nonce range.
    /// The lower bound of nonce range is EPOCH start of day in microseconds,
    /// and upper bound of nonce range is EPOCH end of day in microseconds.
    /// </summary>
    public static Task<BxHttpResponse<Nonce>> GetNonce(this BxHttpClient httpClient)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.Nonce)
            .Build();

        return httpClient.Get<Nonce>(bxPath);
    }

    /// <summary>
    /// Get the current Exchange Time
    /// </summary>
    public static Task<BxHttpResponse<ExchangeTime>> GetTime(this BxHttpClient httpClient)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.Time)
            .Build();

        return httpClient.Get<ExchangeTime>(bxPath);
    }
}
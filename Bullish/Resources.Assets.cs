using Bullish.Internals;
using Bullish.Schemas;

namespace Bullish;

public static partial class Resources
{
    /// <summary>
    /// Get supported assets
    /// </summary>
    public static Task<BxHttpResponse<List<Asset>>> GetAssets(this BxHttpClient httpClient)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.Assets)
            .Build();

        return httpClient.Get<List<Asset>>(bxPath);
    }

    /// <summary>
    /// Get asset by symbol
    /// </summary>
    /// <param name="symbol">For example "BTC"</param>
    public static Task<BxHttpResponse<Asset>> GetAsset(this BxHttpClient httpClient, string symbol)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.AssetsSymbol)
            .AddResourceId(symbol)
            .Build();

        return httpClient.Get<Asset>(bxPath);
    }
}
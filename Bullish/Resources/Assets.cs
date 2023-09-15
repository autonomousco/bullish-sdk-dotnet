using Bullish.BxClient;

namespace Bullish.Resources;

public static class Assets
{
    /// <summary>
    /// Get supported assets
    /// </summary>
    public static async Task<BxHttpResponse<List<Asset>>> GetAssets(this BxHttpClient httpClient)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.Assets)
            .Build();
        
        return await httpClient.Get<List<Asset>>(bxPath);
    }

    /// <summary>
    /// Get asset by symbol
    /// </summary>
    /// <param name="symbol">For example "BTC"</param>
    public static async Task<BxHttpResponse<Asset>> GetAsset(this BxHttpClient httpClient, string symbol)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.AssetsSymbol)
            .AddResourceId(symbol)
            .Build();
        
        return await httpClient.Get<Asset>(bxPath);
    }
}
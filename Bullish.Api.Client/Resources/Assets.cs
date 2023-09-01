using Bullish.Api.Client.HttpClient;

namespace Bullish.Api.Client.Resources;

public static class Assets
{
    /// <summary>
    /// Get supported assets
    /// </summary>
    public static async Task<BxHttpResponse<List<Asset>>> GetAssets(this BxHttpClient httpClient)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.Assets);
        return await httpClient.MakeRequest<List<Asset>>(pathBuilder.Path);
    }

    /// <summary>
    /// Get asset by symbol
    /// </summary>
    /// <param name="symbol">For example "BTC"</param>
    public static async Task<BxHttpResponse<Asset>> GetAsset(this BxHttpClient httpClient, string symbol)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.AssetsSymbol, symbol);
        return await httpClient.MakeRequest<Asset>(pathBuilder.Path);
    }
}
using Bullish.Internals;
using Bullish.Schemas;

namespace Bullish;

public static partial class Resources
{
    /// <summary>
    /// Gets the asset accounts.
    /// </summary>
    public static Task<BxHttpResponse<List<AssetAccount>>> GetAssetAccounts(this BxHttpClient httpClient)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.AccountsAsset)
            .Build();

        return httpClient.Get<List<AssetAccount>>(bxPath);
    }

    /// <summary>
    /// Gets the asset account by symbol.
    /// </summary>
    /// <param name="symbol">For example "BTC"</param>
    public static Task<BxHttpResponse<AssetAccount>> GetAssetAccount(this BxHttpClient httpClient, string symbol)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.AccountsAssetSymbol)
            .AddResourceId(symbol)
            .Build();

        return httpClient.Get<AssetAccount>(bxPath);
    }

    /// <summary>
    /// Gets details for all trading accounts accessible by the API key used in the request.
    /// The trading account ID will be referenced in all other REST API responses.
    /// </summary>
    public static Task<BxHttpResponse<List<TradingAccount>>> GetTradingAccounts(this BxHttpClient httpClient)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.AccountsTradingAccounts)
            .Build();

        return httpClient.Get<List<TradingAccount>>(bxPath);
    }
}
using Bullish.Internals;
using Bullish.Schemas;

namespace Bullish;

public static partial class Resources
{
    /// <summary>
    /// Gets the asset accounts.
    /// </summary>
    public static Task<BxHttpResponse<List<AssetAccount>>> GetAssetAccounts(this BxHttpClient httpClient, string tradingAccountId)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.AccountsAsset)
            .AddQueryParam("tradingAccountId", tradingAccountId)
            .Build();

        return httpClient.Get<List<AssetAccount>>(bxPath);
    }

    /// <summary>
    /// Gets the asset account by symbol.
    /// </summary>
    /// <param name="symbol">For example "BTC"</param>
    /// <param name="tradingAccountId"></param>
    public static Task<BxHttpResponse<AssetAccount>> GetAssetAccount(this BxHttpClient httpClient, string symbol, string tradingAccountId)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.AccountsAssetSymbol)
            .AddResourceId(symbol)
            .AddQueryParam("tradingAccountId", tradingAccountId)
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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tradingAccountId"></param>
    /// <returns></returns>
    public static Task<BxHttpResponse<TradingAccount>> GetTradingAccount(this BxHttpClient httpClient, string tradingAccountId)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.AccountsTradingAccounts)
            .AddResourceId(tradingAccountId)
            .Build();

        return httpClient.Get<TradingAccount>(bxPath);
    }
}
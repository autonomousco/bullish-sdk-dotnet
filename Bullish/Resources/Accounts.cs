using Bullish.BxClient;

namespace Bullish.Resources;

public static class Accounts
{
    /// <summary>
    /// Gets the asset accounts.
    /// </summary>
    public static async Task<BxHttpResponse<List<AssetAccount>>> GetAssetAccounts(this BxHttpClient httpClient)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.AccountsAsset)
            .Build();
        
        return await httpClient.Get<List<AssetAccount>>(bxPath);
    }

    /// <summary>
    /// Gets the asset account by symbol.
    /// </summary>
    /// <param name="symbol">For example "BTC"</param>
    public static async Task<BxHttpResponse<AssetAccount>> GetAssetAccount(this BxHttpClient httpClient, string symbol)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.AccountsAssetSymbol)
            .AddResourceId(symbol)
            .Build();
        
        return await httpClient.Get<AssetAccount>(bxPath);
    }

    /// <summary>
    /// Gets details for all trading accounts accessible by the API key used in the request.
    /// The trading account ID will be referenced in all other REST API responses.
    /// </summary>
    public static async Task<BxHttpResponse<List<TradingAccount>>> GetTradingAccounts(this BxHttpClient httpClient)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.AccountsTradingAccounts)
            .Build();
        
        return await httpClient.Get<List<TradingAccount>>(bxPath);
    }
}
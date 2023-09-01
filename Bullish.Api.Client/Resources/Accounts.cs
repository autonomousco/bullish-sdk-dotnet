using Bullish.Api.Client.HttpClient;

namespace Bullish.Api.Client.Resources;

public static class Accounts
{
    /// <summary>
    /// Gets the asset accounts.
    /// </summary>
    public static async Task<BxHttpResponse<List<AssetAccount>>> GetAssetAccounts(this BxHttpClient httpClient)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.AccountsAsset);
        return await httpClient.MakeRequest<List<AssetAccount>>(pathBuilder.Path);
    }

    /// <summary>
    /// Gets the asset account by symbol.
    /// </summary>
    /// <param name="symbol">For example "BTC"</param>
    public static async Task<BxHttpResponse<AssetAccount>> GetAssetAccount(this BxHttpClient httpClient, string symbol)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.AccountsAssetSymbol, symbol);
        return await httpClient.MakeRequest<AssetAccount>(pathBuilder.Path);
    }

    /// <summary>
    /// Gets details for all trading accounts accessible by the API key used in the request.
    /// The trading account ID will be referenced in all other REST API responses.
    /// </summary>
    public static async Task<BxHttpResponse<List<TradingAccount>>> GetTradingAccounts(this BxHttpClient httpClient)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.AccountsTradingAccounts);
        return await httpClient.MakeRequest<List<TradingAccount>>(pathBuilder.Path);
    }
}
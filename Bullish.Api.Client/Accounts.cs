namespace Bullish.Api.Client;

public static class Accounts
{
    // Get Asset Accounts
    // /accounts/asset
    public static async Task<BxHttpResponse<List<AssetAccount>>> GetAssetAccounts(this BxHttpClient httpClient)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.AccountsAsset);
        return await httpClient.MakeRequest<List<AssetAccount>>(pathBuilder.Path);
    }
    
    // Get Asset Account by Symbol
    // /accounts/asset/{symbol}
    public static async Task<BxHttpResponse<AssetAccount>> GetAssetAccount(this BxHttpClient httpClient, string symbol)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.AccountsAssetSymbol, symbol);
        return await httpClient.MakeRequest<AssetAccount>(pathBuilder.Path);
    }
    
    // Get all trading Accounts details
    // /accounts/trading-accounts
    public static async Task<BxHttpResponse<List<TradingAccount>>> GetTradingAccounts(this BxHttpClient httpClient)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.AccountsTradingAccounts);
        return await httpClient.MakeRequest<List<TradingAccount>>(pathBuilder.Path);
    }
}
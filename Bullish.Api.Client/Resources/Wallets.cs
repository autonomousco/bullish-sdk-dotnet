using Bullish.Api.Client.HttpClient;

namespace Bullish.Api.Client.Resources;

public static class Wallets
{
    /// <summary>
    /// Get custody transaction history
    /// </summary>
    public static async Task<BxHttpResponse<List<WalletTransaction>>> GetTransactions(this BxHttpClient httpClient)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.WalletsTransactions);
        return await httpClient.MakeRequest<List<WalletTransaction>>(pathBuilder.Path);
    }
    
    /// <summary>
    /// Get withdrawal limits for symbol
    /// </summary>
    /// <param name="symbol"></param>
    public static async Task<BxHttpResponse<WalletWithdrawalLimit>> GetWithdrawalLimits(this BxHttpClient httpClient, string symbol = "")
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.WalletsLimitsSymbol)
            .AddQueryParam("symbol", symbol);
        
        return await httpClient.MakeRequest<WalletWithdrawalLimit>(pathBuilder.Path);
    }
    
    /// <summary>
    /// Get Deposit Instructions for Crypto
    /// </summary>
    /// <param name="symbol"></param>
    public static async Task<BxHttpResponse<List<WalletDepositCrypto>>> GetDepositInstructionsCrypto(this BxHttpClient httpClient, string symbol = "")
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.WalletsDepositInstructionsCryptoSymbol)
            .AddQueryParam("symbol", symbol);

        return await httpClient.MakeRequest<List<WalletDepositCrypto>>(pathBuilder.Path);
    }
    
    /// <summary>
    /// Get Withdrawal Instructions for Crypto
    /// Please note that before withdrawal destinations can be used for withdrawing to, they must be whitelisted on the Bullish website.
    /// </summary>
    /// <param name="symbol"></param>
    public static async Task<BxHttpResponse<List<WalletWithdrawalCrypto>>> GetWithdrawalInstructionsCrypto(this BxHttpClient httpClient, string symbol = "")
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.WalletsWithdrawalInstructionsCryptoSymbol)
            .AddQueryParam("symbol", symbol);

        return await httpClient.MakeRequest<List<WalletWithdrawalCrypto>>(pathBuilder.Path);
    }
    
    /// <summary>
    /// Get Deposit Instructions for Fiat
    /// </summary>
    /// <param name="symbol"></param>
    public static async Task<BxHttpResponse<List<WalletDepositFiat>>> GetDepositInstructionsFiat(this BxHttpClient httpClient, string symbol = "")
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.WalletsDepositInstructionsFiatSymbol)
            .AddQueryParam("symbol", symbol);

        return await httpClient.MakeRequest<List<WalletDepositFiat>>(pathBuilder.Path);
    }
    
    /// <summary>
    /// Get withdrawal instructions added by the user.
    /// Please note that before withdrawal destinations can be used for withdrawing to, they must be whitelisted on the Bullish website. 
    /// </summary>
    /// <param name="symbol"></param>
    public static async Task<BxHttpResponse<List<WalletWithdrawalFiat>>> GetWithdrawalInstructionsFiat(this BxHttpClient httpClient, string symbol = "")
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.WalletsWithdrawalInstructionsFiatSymbol)
            .AddQueryParam("symbol", symbol);

        return await httpClient.MakeRequest<List<WalletWithdrawalFiat>>(pathBuilder.Path);
    }
}
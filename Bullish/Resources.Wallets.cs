namespace Bullish;

public static partial class Resources
{
    /// <summary>
    /// Get custody transaction history
    /// </summary>
    /// <param name="pageSize">The number of candles to return 5, 25, 50, 100, default value is 25</param>
    /// <param name="pageLink">Get the results for the next or previous page</param>
    public static async Task<BxHttpResponse<List<WalletTransaction>>> GetTransactions(this BxHttpClient httpClient, int pageSize = 25, BxPageLink? pageLink = null)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.WalletsTransactions)
            .AddPagination(pageSize, useMetaData: true)
            .AddPageLink(pageLink ?? BxPageLink.Empty)
            .Build();
        
        return await httpClient.Get<List<WalletTransaction>>(bxPath);
    }
    
    /// <summary>
    /// Get withdrawal limits for symbol
    /// </summary>
    /// <param name="symbol"></param>
    public static async Task<BxHttpResponse<WalletWithdrawalLimit>> GetWithdrawalLimits(this BxHttpClient httpClient, string symbol = "")
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.WalletsLimitsSymbol)
            .AddResourceId(symbol)
            .Build();
        
        return await httpClient.Get<WalletWithdrawalLimit>(bxPath);
    }
    
    /// <summary>
    /// Get Deposit Instructions for Crypto
    /// </summary>
    /// <param name="symbol"></param>
    public static async Task<BxHttpResponse<List<WalletDepositCrypto>>> GetDepositInstructionsCrypto(this BxHttpClient httpClient, string symbol = "")
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.WalletsDepositInstructionsCryptoSymbol)
            .AddResourceId(symbol)
            .Build();

        return await httpClient.Get<List<WalletDepositCrypto>>(bxPath);
    }
    
    /// <summary>
    /// Get Withdrawal Instructions for Crypto
    /// Please note that before withdrawal destinations can be used for withdrawing to, they must be whitelisted on the Bullish website.
    /// </summary>
    /// <param name="symbol"></param>
    public static async Task<BxHttpResponse<List<WalletWithdrawalCrypto>>> GetWithdrawalInstructionsCrypto(this BxHttpClient httpClient, string symbol = "")
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.WalletsWithdrawalInstructionsCryptoSymbol)
            .AddResourceId(symbol)
            .Build();

        return await httpClient.Get<List<WalletWithdrawalCrypto>>(bxPath);
    }
    
    /// <summary>
    /// Get Deposit Instructions for Fiat
    /// </summary>
    /// <param name="symbol"></param>
    public static async Task<BxHttpResponse<List<WalletDepositFiat>>> GetDepositInstructionsFiat(this BxHttpClient httpClient, string symbol = "")
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.WalletsDepositInstructionsFiatSymbol)
            .AddResourceId(symbol)
            .Build();

        return await httpClient.Get<List<WalletDepositFiat>>(bxPath);
    }
    
    /// <summary>
    /// Get withdrawal instructions added by the user.
    /// Please note that before withdrawal destinations can be used for withdrawing to, they must be whitelisted on the Bullish website. 
    /// </summary>
    /// <param name="symbol"></param>
    public static async Task<BxHttpResponse<List<WalletWithdrawalFiat>>> GetWithdrawalInstructionsFiat(this BxHttpClient httpClient, string symbol = "")
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.WalletsWithdrawalInstructionsFiatSymbol)
            .AddResourceId(symbol)
            .Build();

        return await httpClient.Get<List<WalletWithdrawalFiat>>(bxPath);
    }
}
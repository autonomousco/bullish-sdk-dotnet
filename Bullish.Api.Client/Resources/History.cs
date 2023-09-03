using Bullish.Api.Client.BxClient;

namespace Bullish.Api.Client.Resources;

public static class History
{
    /// <summary>
    /// Get Historical Market Trades by Market Symbol.
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    public static async Task<BxHttpResponse<List<MarketTrade>>> GetMarketTrades(this BxHttpClient httpClient, string symbol)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.HistoryMarketsSymbolTrades)
            .AddResourceId(symbol)
            .Build();
        
        return await httpClient.Get<List<MarketTrade>>(bxPath);
    }
    
    /// <summary>
    /// Get Historical Hourly Borrow Interest. Each entry denotes the hourly quantities for the specific asset.
    /// Total borrowed quantity is inclusive of interest. interest = totalBorrowedQuantity - borrowedQuantity which
    /// denotes the interest charged in the particular hour for the asset.
    /// </summary>
    public static async Task<BxHttpResponse<List<BorrowInterest>>> GetHourlyBorrowInterest(this BxHttpClient httpClient)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.HistoryBorrowInterest)
            .Build();
        
        return await httpClient.Get<List<BorrowInterest>>(bxPath);
    }
}
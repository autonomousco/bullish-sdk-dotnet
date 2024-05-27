using Bullish.Internals;
using Bullish.Schemas;

namespace Bullish;

public static partial class Resources
{
    /// <summary>
    /// Get Historical Market Trades by Market Symbol.
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    /// <param name="pageSize">The number of candles to return 5, 25, 50, 100, default value is 25</param>
    /// <param name="pageLink">Get the results for the next or previous page</param>
    public static Task<BxHttpResponse<List<MarketTrade>>> GetMarketTrades(this BxHttpClient httpClient, string symbol, int pageSize = 25, BxPageLinks.PageLink? pageLink = null)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.HistoryMarketsSymbolTrades)
            .AddResourceId(symbol)
            .AddPagination(pageSize, useMetaData: true)
            .AddPageLink(pageLink ?? BxPageLinks.PageLink.Empty)
            .Build();

        return httpClient.Get<List<MarketTrade>>(bxPath);
    }

    /// <summary>
    /// Get Historical Hourly Borrow Interest. Each entry denotes the hourly quantities for the specific asset.
    /// Total borrowed quantity is inclusive of interest. interest = totalBorrowedQuantity - borrowedQuantity which
    /// denotes the interest charged in the particular hour for the asset.
    /// </summary>
    /// <param name="pageSize">The number of candles to return 5, 25, 50, 100, default value is 25</param>
    /// <param name="pageLink">Get the results for the next or previous page</param>
    public static Task<BxHttpResponse<List<BorrowInterest>>> GetHourlyBorrowInterest(this BxHttpClient httpClient, int pageSize = 25, BxPageLinks.PageLink? pageLink = null)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.HistoryBorrowInterest)
            .AddPagination(pageSize, useMetaData: true)
            .AddPageLink(pageLink ?? BxPageLinks.PageLink.Empty)
            .Build();

        return httpClient.Get<List<BorrowInterest>>(bxPath);
    }
    
    // TODO: Add /history/transfer
    // https://api.exchange.bullish.com/docs/api/rest/trading-api/v2/#get-/history/transfer
}
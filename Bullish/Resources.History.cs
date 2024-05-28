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

    /// <summary>
    /// Get historical transfers.
    /// </summary>
    /// <param name="tradingAccountId">Trading Account ID</param>
    /// <param name="toTimestamp">End datetime of window,</param>
    /// <param name="transferStatus">Status of the transfer request. Defaults to Close</param>
    /// <param name="assetSymbol">Asset symbol of the transfer request</param>
    /// <param name="pageSize">The number of candles to return 5, 25, 50, 100, default value is 25</param>
    /// <param name="pageLink">Get the results for the next or previous page</param>
    /// <param name="requestId">Unique identifier of the transfer request</param>
    /// <param name="fromTimestamp">Start datetime of window,</param>
    public static Task<BxHttpResponse<List<Transfer>>> GetTransfers(this BxHttpClient httpClient,
        string tradingAccountId,
        DateTime fromTimestamp, 
        DateTime toTimestamp,
        TransferStatus transferStatus = TransferStatus.Closed,
        string requestId = "",
        string assetSymbol = "",
        int pageSize = 25,
        BxPageLinks.PageLink? pageLink = null)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.HistoryTransfer)
            .AddPagination(pageSize, useMetaData: true)
            .AddPageLink(pageLink ?? BxPageLinks.PageLink.Empty)
            .AddQueryParam("tradingAccountId", tradingAccountId)
            .AddQueryParam("status", transferStatus)
            .AddQueryParam("requestId", requestId)
            .AddQueryParam("assetSymbol", assetSymbol)
            .AddQueryParam(DateTimeFilter.GreaterThanOrEqual(fromTimestamp))
            .AddQueryParam(DateTimeFilter.LessThanOrEqual(toTimestamp))
            .Build();

        return httpClient.Get<List<Transfer>>(bxPath);
    }
}
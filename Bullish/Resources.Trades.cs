using Bullish.Internals;

namespace Bullish;

public static partial class Resources
{
    /// <summary>
    /// Get a list of trades based on specified filters.
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    /// <param name="orderId">Unique order ID</param>
    /// <param name="pageSize">The number of candles to return 5, 25, 50, 100, default value is 25</param>
    /// <param name="pageLink">Get the results for the next or previous page</param>
    public static async Task<BxHttpResponse<List<Trade>>> GetTrades(this BxHttpClient httpClient, string symbol, string orderId = "", int pageSize = 25, BxPageLinks.PageLink? pageLink = null)
    {
        // TODO: Add date filters
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.Trades)
            .AddQueryParam("symbol", symbol)
            .AddQueryParam("orderId", orderId)
            .AddPagination(pageSize, useMetaData: true)
            .AddPageLink(pageLink ?? BxPageLinks.PageLink.Empty)
            .Build();
        
        return await httpClient.Get<List<Trade>>(bxPath);
    }
    
    /// <summary>
    /// Gets a trade by ID
    /// </summary>
    /// <param name="tradeId">The trade ID</param>
    public static async Task<BxHttpResponse<Trade>> GetTrade(this BxHttpClient httpClient, string tradeId)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.TradesTradeId)
            .AddResourceId(tradeId)
            .Build();
        return await httpClient.Get<Trade>(bxPath);
    }
}
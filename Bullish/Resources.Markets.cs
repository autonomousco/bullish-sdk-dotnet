using Bullish.Internals;
using Bullish.Schemas;

namespace Bullish;

public static partial class Resources
{
    /// <summary>
    /// Get Markets
    /// </summary>
    public static Task<BxHttpResponse<List<Market>>> GetMarkets(this BxHttpClient httpClient)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.Markets)
            .Build();

        return httpClient.Get<List<Market>>(bxPath);
    }

    /// <summary>
    /// Get Market by Symbol
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    public static Task<BxHttpResponse<Market>> GetMarket(this BxHttpClient httpClient, string symbol)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.MarketsSymbol)
            .AddResourceId(symbol)
            .Build();

        return httpClient.Get<Market>(bxPath);
    }

    /// <summary>
    /// Get Order Book by Market Symbol
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    /// <param name="depth">Controls the number of bids/asks returned from the mid price</param>
    public static Task<BxHttpResponse<OrderBook>> GetMarketOrderBook(this BxHttpClient httpClient, string symbol, int depth = 10)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.MarketsSymbolOrderBookHybrid)
            .AddResourceId(symbol)
            .AddQueryParam("depth", depth)
            .Build();

        return httpClient.Get<OrderBook>(bxPath);
    }

    /// <summary>
    /// Get Market Trades by Market Symbol.
    /// Return 100 most recent trades. Lookup from local cache.
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    public static Task<BxHttpResponse<List<MarketTrade>>> GetMarketTrades(this BxHttpClient httpClient, string symbol)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.MarketsSymbolTrades)
            .AddResourceId(symbol)
            .Build();

        return httpClient.Get<List<MarketTrade>>(bxPath);
    }

    /// <summary>
    /// Get Current Tick by Market Symbol.
    /// Return top 100.
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    public static Task<BxHttpResponse<Tick>> GetMarketTick(this BxHttpClient httpClient, string symbol)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.MarketsSymbolTick)
            .AddResourceId(symbol)
            .Build();

        return httpClient.Get<Tick>(bxPath);
    }

    /// <summary>
    /// Get Current OHLCV Candle by Market Symbol
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    /// <param name="timeBucket">Time bucket size</param>
    /// <param name="fromTimestamp">Start timestamp of window</param>
    /// <param name="toTimestamp">End timestamp of window</param>
    /// <param name="pageSize">The number of candles to return 5, 25, 50, 100, default value is 25</param>
    /// <param name="pageLink">Get the results for the next or previous page</param>
    public static Task<BxHttpResponse<List<MarketCandle>>> GetMarketCandles(this BxHttpClient httpClient, string symbol, TimeBucket timeBucket, DateTime fromTimestamp, DateTime toTimestamp, int pageSize = 25, BxPageLinks.PageLink? pageLink = null)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.MarketsSymbolCandle)
            .AddResourceId(symbol)
            .AddQueryParam(DateTimeFilter.GreaterThanOrEqual(fromTimestamp))
            .AddQueryParam(DateTimeFilter.LessThanOrEqual(toTimestamp))
            .AddQueryParam("timeBucket", timeBucket)
            .AddPagination(pageSize, useMetaData: true)
            .AddPageLink(pageLink ?? BxPageLinks.PageLink.Empty)
            .Build();

        return httpClient.Get<List<MarketCandle>>(bxPath);
    }
}
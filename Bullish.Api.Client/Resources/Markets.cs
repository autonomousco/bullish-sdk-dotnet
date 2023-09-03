using Bullish.Api.Client.BxClient;

namespace Bullish.Api.Client.Resources;

public static class Markets
{
    /// <summary>
    /// Get Markets
    /// </summary>
    public static async Task<BxHttpResponse<List<Market>>> GetMarkets(this BxHttpClient httpClient)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.Markets)
            .Build();
        
        return await httpClient.Get<List<Market>>(bxPath);
    }

    /// <summary>
    /// Get Market by Symbol
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    public static async Task<BxHttpResponse<Market>> GetMarket(this BxHttpClient httpClient, string symbol)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.MarketsSymbol)
            .AddResourceId(symbol)
            .Build();
        
        return await httpClient.Get<Market>(bxPath);
    }

    /// <summary>
    /// Get Order Book by Market Symbol
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    /// <param name="depth">Controls the number of bids/asks returned from the mid price</param>
    public static async Task<BxHttpResponse<OrderBook>> GetMarketOrderBook(this BxHttpClient httpClient, string symbol = "BTCUSD", int depth = 10)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.MarketsSymbolOrderBookHybrid)
                .AddResourceId(symbol)
                .AddQueryParam("depth", depth)
                .Build();
        
        return await httpClient.Get<OrderBook>(bxPath);
    }
    
    /// <summary>
    /// Get Market Trades by Market Symbol.
    /// Return 100 most recent trades. Lookup from local cache.
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    public static async Task<BxHttpResponse<List<MarketTrade>>> GetMarketTrades(this BxHttpClient httpClient, string symbol)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.MarketsSymbolTrades)
            .AddResourceId(symbol)
            .Build();
        
        return await httpClient.Get<List<MarketTrade>>(bxPath);
    }
    
    /// <summary>
    /// Get Current Tick by Market Symbol.
    /// Return top 100.
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    public static async Task<BxHttpResponse<Tick>> GetMarketTick(this BxHttpClient httpClient, string symbol = "BTCUSD")
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.MarketsSymbolTick)
            .AddResourceId(symbol)
            .Build();
        
        return await httpClient.Get<Tick>(bxPath);
    }

    /// <summary>
    /// Get Current OHLCV Candle by Market Symbol
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    /// <param name="timeBucket">Time bucket size</param>
    /// <param name="fromTimestamp">Start timestamp of window</param>
    /// <param name="toTimestamp">End timestamp of window</param>
    /// <param name="pageSize">The number of candles to return 5, 25, 50, 100, default value is 25</param>
    public static async Task<BxHttpResponse<List<MarketCandle>>> GetMarketCandles(this BxHttpClient httpClient, string symbol, TimeBucket timeBucket, DateTime fromTimestamp, DateTime toTimestamp, int pageSize = 25, BxPageLink? pageLink = null)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.MarketsSymbolCandle)
            .AddResourceId(symbol)
            .AddQueryParam(BxDateTime.GreaterThanOrEqual(fromTimestamp))
            .AddQueryParam(BxDateTime.LessThanOrEqual(toTimestamp))
            .AddQueryParam("timeBucket", timeBucket)
            .AddPagination(pageSize, useMetaData: true)
            .AddPageLink(pageLink ?? BxPageLink.Empty)
            .Build();

        return await httpClient.Get<List<MarketCandle>>(bxPath);
    }
}
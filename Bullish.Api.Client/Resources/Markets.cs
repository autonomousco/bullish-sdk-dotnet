using Bullish.Api.Client.HttpClient;

namespace Bullish.Api.Client.Resources;

public static class Markets
{
    /// <summary>
    /// Get Markets
    /// </summary>
    public static async Task<BxHttpResponse<List<Market>>> GetMarkets(this BxHttpClient httpClient)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.Markets);
        return await httpClient.MakeRequest<List<Market>>(pathBuilder.Path);
    }

    /// <summary>
    /// Get Market by Symbol
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    public static async Task<BxHttpResponse<Market>> GetMarket(this BxHttpClient httpClient, string symbol)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.MarketsSymbol, symbol);
        return await httpClient.MakeRequest<Market>(pathBuilder.Path);
    }

    /// <summary>
    /// Get Order Book by Market Symbol
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    /// <param name="depth">Controls the number of bids/asks returned from the mid price</param>
    public static async Task<BxHttpResponse<OrderBook>> GetMarketOrderBook(this BxHttpClient httpClient, string symbol = "BTCUSD", int depth = 10)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.MarketsSymbolOrderBookHybrid, symbol)
            .AddQueryParam("depth", depth);
        return await httpClient.MakeRequest<OrderBook>(pathBuilder.Path);
    }
    
    /// <summary>
    /// Get Market Trades by Market Symbol.
    /// Return 100 most recent trades. Lookup from local cache.
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    public static async Task<BxHttpResponse<List<MarketTrade>>> GetMarketTrades(this BxHttpClient httpClient, string symbol)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.MarketsSymbolTrades, symbol);
        return await httpClient.MakeRequest<List<MarketTrade>>(pathBuilder.Path);
    }
    
    /// <summary>
    /// Get Current Tick by Market Symbol.
    /// Return top 100.
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    public static async Task<BxHttpResponse<Tick>> GetMarketTick(this BxHttpClient httpClient, string symbol = "BTCUSD")
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.MarketsSymbolTick, symbol);
        return await httpClient.MakeRequest<Tick>(pathBuilder.Path);
    }

    /// <summary>
    /// Get Current OHLCV Candle by Market Symbol
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    /// <param name="timeBucket">Time bucket size</param>
    /// <param name="fromTimestamp">Start timestamp of window</param>
    /// <param name="toTimestamp">End timestamp of window</param>
    public static async Task<BxHttpResponse<List<MarketCandle>>> GetMarketCandles(this BxHttpClient httpClient, string symbol, TimeBucket timeBucket, DateTime fromTimestamp, DateTime toTimestamp)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.MarketsSymbolTrades, symbol)
            .AddQueryParam(BxDateTime.GreaterThanOrEqual(fromTimestamp))
            .AddQueryParam(BxDateTime.LessThanOrEqual(toTimestamp))
            .AddQueryParam("timeBucket", timeBucket);
        
        return await httpClient.MakeRequest<List<MarketCandle>>(pathBuilder.Path);
    }
}
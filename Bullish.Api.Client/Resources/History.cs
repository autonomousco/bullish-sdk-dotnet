using Bullish.Api.Client.HttpClient;

namespace Bullish.Api.Client.Resources;

public static class History
{
    /// <summary>
    /// Get Historical Market Trades by Market Symbol.
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    public static async Task<BxHttpResponse<List<MarketTrade>>> GetMarketTrades(this BxHttpClient httpClient, string symbol)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.HistoryMarketsSymbolTrades, symbol);
        return await httpClient.MakeRequest<List<MarketTrade>>(pathBuilder.Path);
    }
}
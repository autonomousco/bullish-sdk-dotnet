using Bullish.Api.Client.HttpClient;

namespace Bullish.Api.Client.Resources;

public static class Trades
{
    /// <summary>
    /// Get a list of trades based on specified filters.
    /// </summary>
    /// <param name="symbol">Symbol to get</param>
    /// <param name="orderId">Unique order ID</param>
    public static async Task<BxHttpResponse<List<Trade>>> GetTrades(this BxHttpClient httpClient, string symbol, string orderId = "")
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.MarketsSymbolTrades, symbol)
            .AddQueryParam("orderId", orderId);
        return await httpClient.MakeRequest<List<Trade>>(pathBuilder.Path);
    }
    
    /// <summary>
    /// Gets a trade by ID
    /// </summary>
    /// <param name="tradeId">The trade ID</param>
    public static async Task<BxHttpResponse<Trade>> GetTrade(this BxHttpClient httpClient, string tradeId)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.MarketsSymbolTrades, tradeId)
            .AddQueryParam("tradeId", tradeId);
        return await httpClient.MakeRequest<Trade>(pathBuilder.Path);
    }
}
using Bullish.Api.Client.BxClient;

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
        // TODO: Add date filters
        var bxPath = new BxPathBuilder(BxApiEndpoint.Trades)
            .AddQueryParam("symbol", symbol)
            .AddQueryParam("orderId", orderId)
            .Build();
        
        return await httpClient.Get<List<Trade>>(bxPath);
    }
    
    /// <summary>
    /// Gets a trade by ID
    /// </summary>
    /// <param name="tradeId">The trade ID</param>
    public static async Task<BxHttpResponse<Trade>> GetTrade(this BxHttpClient httpClient, string tradeId)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.TradesTradeId)
            .AddResourceId(tradeId)
            .Build();
        return await httpClient.Get<Trade>(bxPath);
    }
}
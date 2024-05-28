using Bullish.Internals;
using Bullish.Schemas;

namespace Bullish;

public static partial class Resources
{
    /// <summary>
    /// Get derivatives positions
    /// </summary>
    /// <param name="tradingAccountId"></param>
    /// <param name="symbol">Show only positions for this symbol</param>
    /// <returns></returns>
    public static Task<BxHttpResponse<List<Position>>> GetPositions(this BxHttpClient httpClient, string tradingAccountId, string symbol = "")
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.DerivativesPositions)
            .AddQueryParam("tradingAccountId", tradingAccountId)
            .AddQueryParam("symbol", symbol)
            .Build();

        return httpClient.Get<List<Position>>(bxPath);
    }
}
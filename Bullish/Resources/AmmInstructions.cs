using Bullish.BxClient;

namespace Bullish.Resources;

public static class AmmInstructions
{
    /// <summary>
    /// Gets a list of AMM instructions based on applied filters.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="status"></param>
    /// <param name="pageSize">The number of candles to return 5, 25, 50, 100, default value is 25</param>
    /// <param name="pageLink">Get the results for the next or previous page</param>
    public static async Task<BxHttpResponse<List<AmmInstruction>>> GetAmmInstructions(this BxHttpClient httpClient, string symbol = "", AmmInstructionStatus status = AmmInstructionStatus.None, int pageSize = 25, BxPageLink? pageLink = null)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.AmmInstructions)
            .AddQueryParam("symbol", symbol)
            .AddQueryParam("status", status)
            .AddPagination(pageSize, useMetaData: true)
            .AddPageLink(pageLink ?? BxPageLink.Empty)
            .Build();

        return await httpClient.Get<List<AmmInstruction>>(bxPath);
    }
    
    /// <summary>
    /// Gets a specific AMM instruction based on the liquidityId.
    /// </summary>
    /// <param name="liquidityId">Unique AMM instruction ID</param>
    public static async Task<BxHttpResponse<AmmInstruction>> GetAmmInstruction(this BxHttpClient httpClient, string liquidityId)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.AmmInstructionsLiquidityId)
            .AddQueryParam("liquidityId", liquidityId)
            .Build();

        return await httpClient.Get<AmmInstruction>(bxPath);
    }
}
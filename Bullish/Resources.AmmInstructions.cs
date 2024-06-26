using Bullish.Internals;
using Bullish.Schemas;

namespace Bullish;

public static partial class Resources
{
    /// <summary>
    /// Gets a list of AMM instructions based on applied filters.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="status"></param>
    /// <param name="pageSize">The number of instructions to return 5, 25, 50, 100, default value is 25</param>
    /// <param name="pageLink">Get the results for the next or previous page</param>
    public static Task<BxHttpResponse<List<AmmInstruction>>> GetAmmInstructions(this BxHttpClient httpClient, string symbol = "", AmmInstructionStatus status = AmmInstructionStatus.None, int pageSize = 25, BxPageLinks.PageLink? pageLink = null)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.AmmInstructions)
            .AddQueryParam("symbol", symbol)
            .AddQueryParam("status", status)
            .AddPagination(pageSize, useMetaData: true)
            .AddPageLink(pageLink ?? BxPageLinks.PageLink.Empty)
            .Build();

        return httpClient.Get<List<AmmInstruction>>(bxPath);
    }

    /// <summary>
    /// Gets a specific AMM instruction based on the liquidityId.
    /// </summary>
    /// <param name="instructionId">Unique AMM instruction ID</param>
    public static Task<BxHttpResponse<AmmInstruction>> GetAmmInstruction(this BxHttpClient httpClient, string instructionId, string tradingAccountId)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.AmmInstructionsLiquidityId)
            .AddResourceId(instructionId)
            .AddQueryParam("tradingAccountId", tradingAccountId)
            .Build();

        return httpClient.Get<AmmInstruction>(bxPath);
    }
}
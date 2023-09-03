using Bullish.Api.Client.BxClient;

namespace Bullish.Api.Client.Resources;

public static class AmmInstructions
{
    /// <summary>
    /// Gets a list of AMM instructions based on applied filters.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="status"></param>
    public static async Task<BxHttpResponse<List<AmmInstruction>>> GetAmmInstructions(this BxHttpClient httpClient, string symbol = "", AmmInstructionStatus status = AmmInstructionStatus.None)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.AmmInstructions)
            .AddQueryParam("symbol", symbol)
            .AddQueryParam("status", status)
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
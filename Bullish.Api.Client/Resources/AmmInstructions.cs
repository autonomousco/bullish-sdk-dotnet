using Bullish.Api.Client.HttpClient;

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
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.AmmInstructions)
            .AddQueryParam("symbol", symbol)
            .AddQueryParam("status", status);

        return await httpClient.MakeRequest<List<AmmInstruction>>(pathBuilder.Path);
    }
    
    /// <summary>
    /// Gets a specific AMM instruction based on the liquidityId.
    /// </summary>
    /// <param name="liquidityId">Unique AMM instruction ID</param>
    public static async Task<BxHttpResponse<AmmInstruction>> GetAmmInstruction(this BxHttpClient httpClient, string liquidityId)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.AmmInstructionsLiquidityId)
            .AddQueryParam("liquidityId", liquidityId);

        return await httpClient.MakeRequest<AmmInstruction>(pathBuilder.Path);
    }
}
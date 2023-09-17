namespace Bullish;

internal static class ResourcesInternal
{
    /// <summary>
    /// Get the current nonce range.
    /// The lower bound of nonce range is EPOCH start of day in microseconds,
    /// and upper bound of nonce range is EPOCH end of day in microseconds.
    /// </summary>
    public static async Task<BxHttpResponse<BxNonce>> GetNonce(this BxHttpClient httpClient)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.Nonce)
            .Build();
        
        return await httpClient.Get<BxNonce>(bxPath);
    }
    
    /// <summary>
    /// Get the current Exchange Time
    /// </summary>
    public static async Task<BxHttpResponse<ExchangeTime>> GetTime(this BxHttpClient httpClient)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.Time)
            .Build();
        
        return await httpClient.Get<ExchangeTime>(bxPath);
    }
}
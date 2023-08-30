namespace Bullish.Api.Client.Resources;

public static class Markets
{
    // Get Markets
    // /markets
    public static async Task<BxHttpResponse<List<Market>>> GetMarkets(this BxHttpClient httpClient)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.Markets);
        return await httpClient.MakeRequest<List<Market>>(pathBuilder.Path);
    }
    
    public static async Task<BxHttpResponse<Market>> GetMarket(this BxHttpClient httpClient, string symbol)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.MarketsSymbol, symbol);
        return await httpClient.MakeRequest<Market>(pathBuilder.Path);
    }
}
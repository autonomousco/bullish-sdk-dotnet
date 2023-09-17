namespace Bullish.Tests;

public class PublicEndpointTests
{
    [Fact]
    public async Task GetMarkets()
    {
        var bxHttpClient = new BxHttpClient();

        var resp = await bxHttpClient.GetMarkets();
        
        var markets = resp.Result;
         
        Assert.True(resp.IsSuccess);
        Assert.Equal(string.Empty, resp.Error.Message);
        Assert.NotNull(markets);
        Assert.NotEmpty(markets);
    }
    
    [Theory]
    [InlineData("ETHUSDC")]
    [InlineData("BTCUSDC")]
    public async Task GetMarket(string symbol)
    {
        var bxHttpClient = new BxHttpClient();

        var resp = await bxHttpClient.GetMarket(symbol);
        
        var market = resp.Result;
         
        Assert.True(resp.IsSuccess);
        Assert.Equal(string.Empty, resp.Error.Message);
        Assert.NotNull(market);
        
        Assert.Equal("USDC", market.QuoteSymbol);
    }
}
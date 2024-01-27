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
    
    [Theory]
    [InlineData("ETHUSDC")]
    [InlineData("BTCUSDC")]
    public async Task GetOrderBook(string symbol)
    {
        var bxHttpClient = new BxHttpClient();

        var resp = await bxHttpClient.GetMarketOrderBook(symbol);

        var marketOb = resp.Result;

        Assert.True(resp.IsSuccess);
        Assert.Equal(string.Empty, resp.Error.Message);
        Assert.NotNull(marketOb);

        Assert.NotEmpty(marketOb.Asks);
        Assert.NotEmpty(marketOb.Bids);
    }
    
    [Theory]
    [InlineData("ETHUSDC")]
    [InlineData("BTCUSDC")]
    public async Task GetTrades(string symbol)
    {
        var bxHttpClient = new BxHttpClient();

        var resp = await bxHttpClient.GetMarketTrades(symbol);

        var marketTrades = resp.Result;

        Assert.True(resp.IsSuccess);
        Assert.Equal(string.Empty, resp.Error.Message);
        Assert.NotNull(marketTrades);

        Assert.NotEmpty(marketTrades);
    }
}
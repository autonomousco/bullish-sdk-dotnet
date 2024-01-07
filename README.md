# Bullish API Client for .NET

A native .NET API client for the Bullish Exchange API. Documentation for the API can be found [here](https://api.exchange.bullish.com/docs/api/rest/)

## Using the API Client

Add a reference to the Bullish namespace and instantiate the `BxHttpClient` with your API key. If you instantiate `BxHttpClient` without any API keys, only access to public endpoints will work. 

```csharp
// Without API Keys (public endpoints only)
using Bullish;

var bxHttpClient = new BxHttpClient();

var marketResponse = await bxHttpClient.GetMarket("BTCUSDC");

if(marketResponse.IsSuccess)
{
    var market = marketResponse.Result;
    var minNotional = market.MinCostLimit;
}
```

```csharp
// With API Keys
using Bullish;

const string PublicKey = "HMAC-9955...";
const string PrivateKey = "b6526ed3...";

var bxHttpClient = new BxHttpClient(PublicKey, PrivateKey, autoLogin: true);

var resp = await bxHttpClient.Login();

var tradingAccounts = await bxHttpClient.GetTradingAccounts();

var tradingAccount = tradingAccounts.Result.First();

var order = await bxHttpClient.GetOrder("1234567890", tradingAccount.TradingAccountId);
```
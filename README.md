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

var bxHttpClient = new BxHttpClient(PublicKey, PrivateKey);
// Additionally you can specify these optional parameters:
//   MetaData: Only needed if using ECDSA Authentication 
//   AuthMode: Specify an Authentication Mode, either ECDSA or HMAC
//   ApiServer: The Bullish server to use. i.e. Production, Sandbox etc 
//   AutoLogin: Should the client automatically login when a request is made

var resp = await bxHttpClient.Login();

var tradingAccounts = await bxHttpClient.GetTradingAccounts();

var tradingAccount = tradingAccounts.Result.First();

var order = await bxHttpClient.GetOrder("1234567890", tradingAccount.TradingAccountId);
```

## Version History
#### Version 1.2.0 - May 28, 2024
- Added get Derivatives Positions
- Added get Historical Account Transfers
- Added get Historical Hourly Perpetual Settlements
- Update get Asset Account to support Trading ID
- Added Post Only to OrderType
- Added Perpetual to MarketType
- Updated get AMM Instruction to use Instruction ID
- Added get Trading Account
- Added new fields to Asset, Market, Order, Tick and Trade
- Improved error handling when HTTP response content is empty
---
#### Version 1.1.4 - Feb 6, 2024
- Removed legacy BxNonce code to fix invalid nonce error
- Exposed Time and Nonce GET requests
- Added custom JsonConverter for bool data types sent as strings
- Added HMAC and ECDSA credential config via appsettings.json for authenticated integration tests
---
#### Version 1.1.3 - Jan 27, 2024
- Added ECDSA signing for trading commands
---
#### Version 1.1.2 - Jan 27, 2024
- Added support for ECDSA login
- Updated NuGet package references
- Added order book and market trades tests
- Removed redundant async await modifiers from API calls
- Automatically logout if JWT session exists when logging in
---
#### Version 1.1.1 - Jan 8, 2024
- Fixed AverageFillPrice null on rejected order throwing a deserialization exception
---
#### Version 1.1.0 - Jan 8, 2024
- Updated to .NET 8.0
- Deprecated EOS Signer
- Added support for V2 HMAC signing 
- Updated OrderType enum
- Updated Orders, AMMInstructions and Commands to match new V2 API
---
#### Version 1.0.1 - Oct 9, 2023
- Added HMAC support
- Added CancelOrder command
- Fixed Nonce not initializing with current UnixMs
- Updated Commands to fix JSON serialization field ordering
- Fixed TradingAccount schema to support nullable Maker/Taker Fees
- Made BxHttpResponse TResult not nullable for NRT compatibility
---
#### Version 1.0.0 - Sep 20, 2023
- Initial Alpha Release
---
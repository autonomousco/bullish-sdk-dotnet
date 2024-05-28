namespace Bullish.Internals;

internal static class Constants
{
    public static readonly Dictionary<BxApiServer, string> BxApiServers = new()
    {
        { BxApiServer.Production, "https://api.exchange.bullish.com/trading-api" },
        { BxApiServer.ProductionRegistered, "https://registered.api.exchange.bullish.com/trading-api" },
        { BxApiServer.Sandbox, "https://api.bugbounty.bullish.com/trading-api" },
        { BxApiServer.SecuritySandbox, "https://api.simnext.bullish-test.com/trading-api" },
        { BxApiServer.SandboxRegistered, "https://registered.api.simnext.bullish-test.com/trading-api" },
    };

    public static readonly Dictionary<BxApiEndpoint, Endpoint> BxApiEndpoints = new()
    {
        // { BxApiEndpoint.Login, new Endpoint("/users/login", "/v2", false) }, // Old EOS style authentication, deprecated March 2024
        { BxApiEndpoint.LoginEcdsa, new Endpoint("/users/login", "/v2", false) },
        { BxApiEndpoint.LoginHmac, new Endpoint("/users/hmac/login", "/v1", false) },
        { BxApiEndpoint.Logout, new Endpoint("/users/logout", "/v1", true) },
        { BxApiEndpoint.Nonce, new Endpoint("/nonce", "/v1", false) },
        { BxApiEndpoint.Time, new Endpoint("/time", "/v1", false) },
        { BxApiEndpoint.AccountsAsset, new Endpoint("/accounts/asset", "/v1", true) },
        { BxApiEndpoint.AccountsAssetSymbol, new Endpoint("/accounts/asset/{symbol}", "/v1", true) },
        { BxApiEndpoint.AccountsTradingAccounts, new Endpoint("/accounts/trading-accounts", "/v1", true) },
        { BxApiEndpoint.DerivativesPositions, new Endpoint("/derivatives-positions", "/v1", true) },
        { BxApiEndpoint.Orders, new Endpoint("/orders", "/v2", true) },
        { BxApiEndpoint.OrdersOrderId, new Endpoint("/orders/{orderId}", "/v2", true) },
        { BxApiEndpoint.AmmInstructions, new Endpoint("/amm-instructions", "/v2", true) },
        { BxApiEndpoint.AmmInstructionsLiquidityId, new Endpoint("/amm-instructions/{instructionid}", "/v2", true) },
        { BxApiEndpoint.WalletsTransactions, new Endpoint("/wallets/transactions", "/v1", true) },
        { BxApiEndpoint.WalletsLimitsSymbol, new Endpoint("/wallets/limits/{symbol}", "/v1", true) },
        { BxApiEndpoint.WalletsDepositInstructionsCryptoSymbol, new Endpoint("/wallets/deposit-instructions/crypto/{symbol}", "/v1", true) },
        { BxApiEndpoint.WalletsWithdrawalInstructionsCryptoSymbol, new Endpoint("/wallets/withdrawal-instructions/crypto/{symbol}", "/v1", true) },
        { BxApiEndpoint.WalletsDepositInstructionsFiatSymbol, new Endpoint("/wallets/deposit-instructions/fiat/{symbol}", "/v1", true) },
        { BxApiEndpoint.WalletsWithdrawalInstructionsFiatSymbol, new Endpoint("/wallets/withdrawal-instructions/fiat/{symbol}", "/v1", true) },
        { BxApiEndpoint.Trades, new Endpoint("/trades", "/v1", true) },
        { BxApiEndpoint.TradesTradeId, new Endpoint("/trades/{tradeId}", "/v1", true) },
        { BxApiEndpoint.Assets, new Endpoint("/assets", "/v1", false) },
        { BxApiEndpoint.AssetsSymbol, new Endpoint("/assets/{symbol}", "/v1", false) },
        { BxApiEndpoint.Markets, new Endpoint("/markets", "/v1", false) },
        { BxApiEndpoint.MarketsSymbol, new Endpoint("/markets/{symbol}", "/v1", false) },
        { BxApiEndpoint.MarketsSymbolOrderBookHybrid, new Endpoint("/markets/{symbol}/orderbook/hybrid", "/v1", false) },
        { BxApiEndpoint.MarketsSymbolTrades, new Endpoint("/markets/{symbol}/trades", "/v1", false) },
        { BxApiEndpoint.HistoryMarketsSymbolTrades, new Endpoint("/history/markets/{symbol}/trades", "/v1", false) },
        { BxApiEndpoint.HistoryBorrowInterest, new Endpoint("/history/borrow-interest", "/v1", true) },
        { BxApiEndpoint.MarketsSymbolTick, new Endpoint("/markets/{symbol}/tick", "/v1", false) },
        { BxApiEndpoint.MarketsSymbolCandle, new Endpoint("/markets/{symbol}/candle", "/v1", false) },
        { BxApiEndpoint.Command, new Endpoint("/command", "/v2", true) },
    };
}
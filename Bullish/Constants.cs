namespace Bullish;

internal static class Constants
{
    public static Dictionary<BxApiServer, string> BxApiServers = new()
    {
        { BxApiServer.Production, "https://api.exchange.bullish.com/trading-api" },
        { BxApiServer.ProductionRegistered, "https://registered.api.exchange.bullish.com/trading-api" },
        { BxApiServer.Sandbox, "https://api.bugbounty.bullish.com/trading-api" },
        { BxApiServer.SecuritySandbox, "https://api.simnext.bullish-test.com/trading-api" },
        { BxApiServer.SandboxRegistered, "https://registered.api.simnext.bullish-test.com/trading-api" },
    };

    public static Dictionary<BxApiEndpoint, BxEndpoint> BxApiEndpoints = new()
    {
        { BxApiEndpoint.Login, new BxEndpoint("/users/login", "/v2", false) },
        { BxApiEndpoint.Logout, new BxEndpoint("/users/logout", "/v1", true) },
        { BxApiEndpoint.Nonce, new BxEndpoint("/nonce", "/v1", false) },
        { BxApiEndpoint.Time, new BxEndpoint("/time", "/v1", false) },
        { BxApiEndpoint.AccountsAsset, new BxEndpoint("/accounts/asset", "/v1", true) },
        { BxApiEndpoint.AccountsAssetSymbol, new BxEndpoint("/accounts/asset/{symbol}", "/v1", true) },
        { BxApiEndpoint.AccountsTradingAccounts, new BxEndpoint("/accounts/trading-accounts", "/v1", true) },
        { BxApiEndpoint.Orders, new BxEndpoint("/orders", "/v1", true) },
        { BxApiEndpoint.OrdersOrderId, new BxEndpoint("/orders/{orderId}", "/v1", true) },
        { BxApiEndpoint.AmmInstructions, new BxEndpoint("/amm-instructions", "/v1", true) },
        { BxApiEndpoint.AmmInstructionsLiquidityId, new BxEndpoint("/amm-instructions/{liquidityId}", "/v1", true) },
        { BxApiEndpoint.WalletsTransactions, new BxEndpoint("/wallets/transactions", "/v1", true) },
        { BxApiEndpoint.WalletsLimitsSymbol, new BxEndpoint("/wallets/limits/{symbol}", "/v1", true) },
        { BxApiEndpoint.WalletsDepositInstructionsCryptoSymbol, new BxEndpoint("/wallets/deposit-instructions/crypto/{symbol}", "/v1", true) },
        { BxApiEndpoint.WalletsWithdrawalInstructionsCryptoSymbol, new BxEndpoint("/wallets/withdrawal-instructions/crypto/{symbol}", "/v1", true) },
        { BxApiEndpoint.WalletsDepositInstructionsFiatSymbol, new BxEndpoint("/wallets/deposit-instructions/fiat/{symbol}", "/v1", true) },
        { BxApiEndpoint.WalletsWithdrawalInstructionsFiatSymbol, new BxEndpoint("/wallets/withdrawal-instructions/fiat/{symbol}", "/v1", true) },
        { BxApiEndpoint.Trades, new BxEndpoint("/trades", "/v1", true) },
        { BxApiEndpoint.TradesTradeId, new BxEndpoint("/trades/{tradeId}", "/v1", true) },
        { BxApiEndpoint.Assets, new BxEndpoint("/assets", "/v1", false) },
        { BxApiEndpoint.AssetsSymbol, new BxEndpoint("/assets/{symbol}", "/v1", false) },
        { BxApiEndpoint.Markets, new BxEndpoint("/markets", "/v1", false) },
        { BxApiEndpoint.MarketsSymbol, new BxEndpoint("/markets/{symbol}", "/v1", false) },
        { BxApiEndpoint.MarketsSymbolOrderBookHybrid, new BxEndpoint("/markets/{symbol}/orderbook/hybrid", "/v1", false) },
        { BxApiEndpoint.MarketsSymbolTrades, new BxEndpoint("/markets/{symbol}/trades", "/v1", false) },
        { BxApiEndpoint.HistoryMarketsSymbolTrades, new BxEndpoint("/history/markets/{symbol}/trades", "/v1", false) },
        { BxApiEndpoint.HistoryBorrowInterest, new BxEndpoint("/history/borrow-interest", "/v1", true) },
        { BxApiEndpoint.MarketsSymbolTick, new BxEndpoint("/markets/{symbol}/tick", "/v1", false) },
        { BxApiEndpoint.MarketsSymbolCandle, new BxEndpoint("/markets/{symbol}/candle", "/v1", false) },
        { BxApiEndpoint.CommandCancelAllOpenOrders, new BxEndpoint("/command?commandType=V1CancelAllOrders", "/v1", true) },
        { BxApiEndpoint.CommandCancelAllOpenOrdersByMarket, new BxEndpoint("/command?commandType=V1CancelAllOrdersByMarket", "/v1", true) },
    };
}
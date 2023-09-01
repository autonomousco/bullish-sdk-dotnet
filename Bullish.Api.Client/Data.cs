namespace Bullish.Api.Client;

public enum OrderSide
{
    None = 0,
    Buy,
    Sell
}

public enum AmmInstructionStatus
{
    None,
    Open,
    Closed,
}

public enum OrderStatus
{
    None = 0,
    Open,
    Closed,
    Cancelled,
    Rejected,
}

public enum TimeBucket
{
    OneMinute,
    FiveMinutes,
    ThirtyMinutes,
    OneHour,
    SixHours,
    TwelveHours,
    OneDay,
}

public enum BxApiServer
{
    Production,
    ProductionRegistered,
    Sandbox,
    SecuritySandbox,
    SandboxRegistered,
}

public enum BxApiEndpoint
{
    Nonce,
    Time,
    AccountsTradingAccounts,
    AccountsAsset,
    AccountsAssetSymbol,
    Orders,
    OrdersOrderId,
    AmmInstructions,
    AmmInstructionsLiquidityId,
    WalletsTransactions,
    WalletsLimitsSymbol,
    WalletsDepositInstructionsCryptoSymbol,
    WalletsWithdrawalInstructionsCryptoSymbol,
    WalletsDepositInstructionsFiatSymbol,
    WalletsWithdrawalInstructionsFiatSymbol,
    Trades,
    TradesTradeId,
    Assets,
    AssetsSymbol,
    Markets,
    MarketsSymbol,
    MarketsSymbolOrderBookHybrid,
    MarketsSymbolTrades,
    HistoryMarketsSymbolTrades,
    MarketsSymbolTick,
    MarketsSymbolCandle,
}

public static class Data
{
    public static Dictionary<BxApiServer, string> BxApiServers = new()
    {
        { BxApiServer.Production, "https://api.exchange.bullish.com/trading-api/v1" },
        { BxApiServer.ProductionRegistered, "https://registered.api.exchange.bullish.com/trading-api/v1" },
        { BxApiServer.Sandbox, "https://api.bugbounty.bullish.com/trading-api/v1" },
        { BxApiServer.SecuritySandbox, "https://api.simnext.bullish-test.com/trading-api/v1" },
        { BxApiServer.SandboxRegistered, "https://registered.api.simnext.bullish-test.com/trading-api/v1" },
    };

    public static Dictionary<BxApiEndpoint, string> BxApiEndpoints = new()
    {
        { BxApiEndpoint.Nonce, "/nonce" },
        { BxApiEndpoint.Time, "/time" },
        { BxApiEndpoint.AccountsAsset, "/accounts/asset" },
        { BxApiEndpoint.AccountsAssetSymbol, "/accounts/asset/{symbol}" },
        { BxApiEndpoint.AccountsTradingAccounts, "/accounts/trading-accounts" },
        { BxApiEndpoint.Orders, "/orders" },
        { BxApiEndpoint.OrdersOrderId, "/orders/{orderId}" },
        { BxApiEndpoint.AmmInstructions, "/amm-instructions" },
        { BxApiEndpoint.AmmInstructionsLiquidityId, "/amm-instructions/{liquidityId}" },
        { BxApiEndpoint.WalletsTransactions, "/wallets/transactions" },
        { BxApiEndpoint.WalletsLimitsSymbol, "/wallets/limits/{symbol}" },
        { BxApiEndpoint.WalletsDepositInstructionsCryptoSymbol, "/wallets/deposit-instructions/crypto/{symbol}" },
        { BxApiEndpoint.WalletsWithdrawalInstructionsCryptoSymbol, "/wallets/withdrawal-instructions/crypto/{symbol}" },
        { BxApiEndpoint.WalletsDepositInstructionsFiatSymbol, "/wallets/deposit-instructions/fiat/{symbol}" },
        { BxApiEndpoint.WalletsWithdrawalInstructionsFiatSymbol, "/wallets/withdrawal-instructions/fiat/{symbol}" },
        { BxApiEndpoint.Trades, "/trades" },
        { BxApiEndpoint.TradesTradeId, "/trades/{tradeId}" },
        { BxApiEndpoint.Assets, "/assets" },
        { BxApiEndpoint.AssetsSymbol, "/assets/{symbol}" },
        { BxApiEndpoint.Markets, "/markets" },
        { BxApiEndpoint.MarketsSymbol, "/markets/{symbol}" },
        { BxApiEndpoint.MarketsSymbolOrderBookHybrid, "/markets/{symbol}/orderbook/hybrid" },
        { BxApiEndpoint.MarketsSymbolTrades, "/markets/{symbol}/trades" },
        { BxApiEndpoint.HistoryMarketsSymbolTrades, "/history/markets/{symbol}/trades" },
        { BxApiEndpoint.MarketsSymbolTick, "/markets/{symbol}/tick" },
        { BxApiEndpoint.MarketsSymbolCandle, "/markets/{symbol}/candle" },
    };
}
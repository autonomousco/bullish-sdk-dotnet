namespace Bullish;

public enum PrecisionType
{
    BasePrecision,
    QuotePrecision,
}

public enum OrderSide
{
    None = 0,
    Buy,
    Sell
}

public enum OrderType
{
    None = 0,
    Lmt,
    Mkt,
    STOP_LIMIT,
}

public enum OrderTimeInForce
{
    None = 0,
    Gtc,
    Fok,
    Ioc,
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
    Login,
    Logout,
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
    HistoryBorrowInterest,
    MarketsSymbolTick,
    MarketsSymbolCandle,
    CommandCancelAllOpenOrders,
    CommandCancelAllOpenOrdersByMarket,
}
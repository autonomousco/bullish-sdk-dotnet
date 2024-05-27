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
    Limit,
    Market,
    Stop_Limit,
    Post_Only,
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

public enum MarketType
{
    None,
    Spot,
    Perpetual,
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

public enum AuthMode
{
    Hmac,
    Ecdsa,
    //Eos,
}

public enum BxApiServer
{
    Production,
    ProductionRegistered,
    Sandbox,
    SecuritySandbox,
    SandboxRegistered,
}

internal enum BxApiEndpoint
{
    Login,
    LoginHmac,
    LoginEcdsa,
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
    Command,
}
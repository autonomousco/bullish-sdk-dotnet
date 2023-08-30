namespace Bullish.Api.Client;

public enum OrderSide
{
    None = 0,
    Buy,
    Sell
}

public enum OrderStatus
{
    None = 0,
    Open,
    Closed,
    Cancelled,
    Rejected,
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
    AccountsTradingAccounts,
    AccountsAsset,
    AccountsAssetSymbol,
    Orders,
    AmmInstructions,
    WalletsTransactions,
    WalletsLimits,
    WalletsDepositInstructionsCrypto,
    WalletsWithdrawalInstructionsCrypto,
    WalletsDepositInstructionsFiat,
    WalletsWithdrawalInstructionsFiat,
    Trades,
    Time,
    Assets,
    Markets,
    MarketsOrderBookHybrid,
    MarketsTrades,
    HistoryMarketsTrades,
    MarketsTick,
    MarketsCandle,
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
        { BxApiEndpoint.AccountsAsset, "/accounts/asset" },
        { BxApiEndpoint.AccountsAssetSymbol, "/accounts/asset/{symbol}" },
        { BxApiEndpoint.AccountsTradingAccounts, "/accounts/trading-accounts" },
        { BxApiEndpoint.Orders, "/orders" },
    };
}
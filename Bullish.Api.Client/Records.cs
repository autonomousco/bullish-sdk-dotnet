using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("ApiTester")]

namespace Bullish.Api.Client;

public record BxPath(BxApiEndpoint Endpoint, string Path, bool UseAuth);

public record BxEndpoint(string Path, string Version, bool UseAuth);

public record BxMetadata
{
    public required string UserId { get; init; }
    public required string PublicKey { get; init; }
    public required string CredentialId { get; init; }
}

public record LoginPayload
{
    public required string UserId { get; init; }
    public required long Nonce { get; init; }
    public required long ExpirationTime { get; init; }
    public required bool BiometricsUsed { get; init; }
    public required string? SessionKey { get; init; }
}

public record Login
{
    public required string PublicKey { get; init; }
    public required string Signature { get; init; }
    public required LoginPayload LoginPayload { get; init; }
}

public record LoginResponse
{
    public required string Authorizer { get; init; }
    public required string OwnerAuthorizer { get; init; }
    public required string Token { get; init; }
}

public record LogoutResponse;

public record BorrowInterest
{
    public required string AssetId { get; set; }
    public required string AssetSymbol { get; set; }
    public required string BorrowedQuantity { get; set; }
    public required string TotalBorrowedQuantity { get; set; }
    public required DateTime CreatedAtDatetime { get; set; }
    public required string CreatedAtTimestamp { get; set; }
}

public record Trade
{
    public required string TradeId { get; init; }
    public required string OrderId { get; init; }
    public required string Symbol { get; init; }
    public required string Price { get; init; }
    public required string Quantity { get; init; }
    public required string BaseFee { get; init; }
    public required string QuoteFee { get; init; }
    public required string Side { get; init; }
    public required bool IsTaker { get; init; }
    public required DateTime CreatedAtDatetime { get; init; }
    public required string CreatedAtTimestamp { get; init; }
}

public record AssetAccount
{
    public required string TradingAccountId { get; init; }
    public required string AssetId { get; init; }
    public required string AssetSymbol { get; init; }
    public required string AvailableQuantity { get; init; }
    public required string BorrowedQuantity { get; init; }
    public required string LockedQuantity { get; init; }
    public required string LoanedQuantity { get; init; }
    public required DateTime UpdatedAtDatetime { get; init; }
    public required string UpdatedAtTimestamp { get; init; }
}

public record Order
{
    public required string Handle { get; init; }
    public required string OrderId { get; init; }
    public required string Symbol { get; init; }
    public required string Price { get; init; }
    public required string AverageFillPrice { get; init; }
    public required string StopPrice { get; init; }
    public required bool Margin { get; init; }
    public required string Quantity { get; init; }
    public required string QuantityFilled { get; init; }
    public required string BaseFee { get; init; }
    public required string QuoteFee { get; init; }
    public required string BorrowedQuantity { get; init; }
    public required bool IsLiquidation { get; init; }
    public required string Side { get; init; }
    public required string Type { get; init; }
    public required string TimeInForce { get; init; }
    public required string Status { get; init; }
    public required string StatusReason { get; init; }
    public required string StatusReasonCode { get; init; }
    public required DateTime CreatedAtDatetime { get; init; }
    public required string CreatedAtTimestamp { get; init; }
}

public record Ask
{
    public required string Price { get; init; }
    public required string PriceLevelQuantity { get; init; }
}

public record Bid
{
    public required string Price { get; init; }
    public required string PriceLevelQuantity { get; init; }
}

public record OrderBook
{
    public required List<Bid> Bids { get; init; }
    public required List<Ask> Asks { get; init; }
    public required DateTime Datetime { get; init; }
    public required string Timestamp { get; init; }
    public required int SequenceNumber { get; init; }
}

public record AmmData
{
    public required string FeeTierId { get; init; }
    public required string BidSpreadFee { get; init; }
    public required string AskSpreadFee { get; init; }
    public required string BaseReservesQuantity { get; init; }
    public required string QuoteReservesQuantity { get; init; }
    public required string CurrentPrice { get; init; }
}

public record Tick
{
    public required DateTime CreatedAtDatetime { get; init; }
    public required string CreatedAtTimestamp { get; init; }
    public required string High { get; init; }
    public required string Low { get; init; }
    public required string BestBid { get; init; }
    public required string BidVolume { get; init; }
    public required string BestAsk { get; init; }
    public required string AskVolume { get; init; }
    public required string Vwap { get; init; }
    public required string Open { get; init; }
    public required string Close { get; init; }
    public required string Last { get; init; }
    public required string Change { get; init; }
    public required string Percentage { get; init; }
    public required string Average { get; init; }
    public required string BaseVolume { get; init; }
    public required string QuoteVolume { get; init; }
    public required string BancorPrice { get; init; }
    public required DateTime LastTradeDatetime { get; init; }
    public required string LastTradeTimestamp { get; init; }
    public required string LastTradeQuantity { get; init; }
    public required List<AmmData> AmmData { get; init; }
}

public record TradingAccount
{
    public required string IsBorrowing { get; init; }
    public required string IsLending { get; init; }
    public required string MakerFee { get; init; }
    public required string TakerFee { get; init; }
    public required string MaxInitialLeverage { get; init; }
    public required string TradingAccountId { get; init; }
    public required string TradingAccountName { get; init; }
    public required string TradingAccountDescription { get; init; }
    public required string IsPrimaryAccount { get; init; }
    public required string RateLimitToken { get; init; }
    public required string IsDefaulted { get; init; }
}

public record FeeTier
{
    public required string FeeTierId { get; init; }
    public required string StaticSpreadFee { get; init; }
    public required bool IsDislocationEnabled { get; init; }
}

public record Market
{
    public required string MarketId { get; init; }
    public required string Symbol { get; init; }
    public required string BaseSymbol { get; init; }
    public required string QuoteSymbol { get; init; }
    public required string QuoteAssetId { get; init; }
    public required string BaseAssetId { get; init; }
    public required int QuotePrecision { get; init; }
    public required int BasePrecision { get; init; }
    public required int PricePrecision { get; init; }
    public required int QuantityPrecision { get; init; }
    public required int CostPrecision { get; init; }
    public required string MinQuantityLimit { get; init; }
    public required string MaxQuantityLimit { get; init; }
    public required string MaxPriceLimit { get; init; }
    public required string MinPriceLimit { get; init; }
    public required string MaxCostLimit { get; init; }
    public required string MinCostLimit { get; init; }
    public required string TimeZone { get; init; }
    public required string TickSize { get; init; }
    public required string LiquidityTickSize { get; init; }
    public required int LiquidityPrecision { get; init; }
    public required int MakerFee { get; init; }
    public required int TakerFee { get; init; }
    public required List<string> OrderTypes { get; init; }
    public required bool SpotTradingEnabled { get; init; }
    public required bool MarginTradingEnabled { get; init; }
    public required bool MarketEnabled { get; init; }
    public required bool CreateOrderEnabled { get; init; }
    public required bool CancelOrderEnabled { get; init; }
    public required List<FeeTier> FeeTiers { get; init; }
}

public record MarketCandle
{
    public required string Open { get; init; }
    public required string High { get; init; }
    public required string Low { get; init; }
    public required string Close { get; init; }
    public required string Volume { get; init; }
    public required string CreatedAtTimestamp { get; init; }
    public required DateTime CreatedAtDatetime { get; init; }
}

public record MarketTrade
{
    public required string TradeId { get; init; }
    public required string Symbol { get; init; }
    public required string Price { get; init; }
    public required string Quantity { get; init; }
    public required string Side { get; init; }
    public required bool IsTaker { get; init; }
    public required DateTime CreatedAtDatetime { get; init; }
    public required string CreatedAtTimestamp { get; init; }
}

public record Asset
{
    public required string AssetId { get; init; }
    public required string Symbol { get; init; }
    public required string Precision { get; init; }
    public required string MinBalanceInterest { get; init; }
    public required string MinFee { get; init; }
    public required string Apr { get; init; }
    public required string CollateralRating { get; init; }
    public required string MaxBorrow { get; init; }
}

public record ExchangeTime
{
    public required long Timestamp { get; init; }
    public required DateTime DateTime { get; init; }
}

public record WalletWithdrawalLimit
{
    public required string Symbol { get; init; }
    public required string Available { get; init; }
    public required string TwentyFourHour { get; init; }
}
public record WalletDepositCrypto
{
    public required string Network { get; set; }
    public required string Symbol { get; set; }
    public required string Address { get; set; }
}

public record WalletWithdrawalCrypto
{
    public required string Network { get; set; }
    public required string Symbol { get; set; }
    public required string Address { get; set; }
    public required string Fee { get; set; }
    public required string Memo { get; set; }
    public required string Label { get; set; }
    public required string DestinationId { get; set; }
}

public record IntermediaryBank
{
    public required string Name { get; set; }
    public required string PhysicalAddress { get; set; }
    public required string RoutingCode { get; set; }
}

public record WalletWithdrawalFiat
{
    public required string DestinationId { get; set; }
    public required string AccountNumber { get; set; }
    public required string Network { get; set; }
    public required string Symbol { get; set; }
    public required string Name { get; set; }
    public required string PhysicalAddress { get; set; }
    public required string Fee { get; set; }
    public required string Memo { get; set; }
    public required Bank Bank { get; set; }
    public required IntermediaryBank IntermediaryBank { get; set; }
}

public record Bank
{
    public required string Name { get; set; }
    public required string PhysicalAddress { get; set; }
    public required string RoutingCode { get; set; }
}

public record WalletDepositFiat
{
    public required string Network { get; set; }
    public required string Symbol { get; set; }
    public required string AccountNumber { get; set; }
    public required string Name { get; set; }
    public required string PhysicalAddress { get; set; }
    public required string Memo { get; set; }
    public required Bank Bank { get; set; }
}

public record WalletTransaction
{
    public required string CustodyTransactionId { get; init; }
    public required string Direction { get; init; }
    public required string Quantity { get; init; }
    public required string Symbol { get; init; }
    public required string Network { get; init; }
    public required string Fee { get; init; }
    public required string Memo { get; init; }
    public required DateTime CreatedAtDateTime { get; init; }
    public required string Status { get; init; }
    public required TransactionDetails TransactionDetails { get; init; }
}

public record TransactionDetails
{
    public required string Address { get; init; }
    public required string BlockchainTxId { get; init; }
    public required string SwiftUetr { get; init; }
}

public record AmmInstruction
{
    public required string LiquidityId { get; init; }
    public required string Symbol { get; init; }
    public required string BaseFee { get; init; }
    public required string QuoteFee { get; init; }
    public required string Status { get; init; }
    public required string StatusReason { get; init; }
    public required int StatusReasonCode { get; init; }
    public required DateTime CreatedAtDatetime { get; init; }
    public required string CreatedAtTimestamp { get; init; }
 
    [JsonPropertyName("24HrApy")]
    public required string TwentyFourHrApy { get; init; }

    [JsonPropertyName("24HrYieldEarn")]
    public required string TwentyFourHrYieldEarn { get; init; }
    
    public required string Apy { get; init; }
    public required string BaseCurrentQuantity { get; init; }
    public required string BaseInvestQuantity { get; init; }
    public required string BasePrice { get; init; }
    public required string BaseWithdrawQuantity { get; init; }
    public required string CurrentValue { get; init; }
    public required bool DislocationEnabled { get; init; }
    public required string FeeTierId { get; init; }
    public required string FinalValue { get; init; }
    public required string ImpermanentLoss { get; init; }
    public required string InitialBasePrice { get; init; }
    public required string InitialQuotePrice { get; init; }
    public required string InitialValue { get; init; }
    public required string LowerBound { get; init; }
    public required string Price { get; init; }
    public required string QuoteCurrentQuantity { get; init; }
    public required string QuoteInvestQuantity { get; init; }
    public required string QuotePrice { get; init; }
    public required string QuoteWithdrawQuantity { get; init; }
    public required string RequestId { get; init; }
    public required string StaticSpreadFee { get; init; }
    public required DateTime UpdatedAtDatetime { get; init; }
    public required string UpdatedAtTimestamp { get; init; }
    public required string UpperBound { get; init; }
    public required string YieldEarn { get; init; }
}


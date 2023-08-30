using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ApiTester")]

namespace Bullish.Api.Client;

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


public record Nonce
{
    public required long LowerBound { get; init; }
    public required long UpperBound { get; init; }

    public long Value { get; private set; } = -1;

    public static Nonce Empty => new Nonce
    {
        UpperBound = 0,
        LowerBound = 0,
    };

    public long NextValue()
    {
        if (Value == -1)
        {
            Value = LowerBound;
            return Value;
        }

        if (Value == UpperBound)
            throw new Exception("Value cannot exceed upper bounds");

        return ++Value;
    }
}

public record BxHttpError
{
    public required string ErrorCode { get; init; }
    public required string ErrorCodeName { get; init; }
    public required string Message { get; init; }
    public required object Raw { get; init; }

    public static BxHttpError Empty => new()
    {
        ErrorCode = string.Empty,
        ErrorCodeName = string.Empty,
        Message = string.Empty,
        Raw = new object(),
    };

    public static BxHttpError Error(string message) => new()
    {
        Message = message,
        Raw = new object(),
        ErrorCode = string.Empty,
        ErrorCodeName = string.Empty,
    };
}

public record BxHttpResponse<T>
{
    public required bool IsSuccess { get; init; }
    public required T? Result { get; init; }
    public required BxHttpError Error { get; init; }

    public static BxHttpResponse<T> Success(T result) => new()
    {
        Result = result,
        IsSuccess = true,
        Error = BxHttpError.Empty
    };

    public static BxHttpResponse<T> Failure(BxHttpError error) => new()
    {
        Result = default,
        IsSuccess = false,
        Error = error
    };

    public static BxHttpResponse<T> Failure(string message) => new()
    {
        Result = default,
        IsSuccess = false,
        Error = BxHttpError.Error(message)
    };
}
using System.Globalization;
using Bullish.Internals;
using Bullish.Schemas;

namespace Bullish;

public static partial class Resources
{
    /// <summary>
    /// Gets the orders list based on specified filters.
    /// </summary>
    /// <param name="symbol">Order symbol</param>
    /// <param name="handle">Unique numeric identifier generated on the client side expressed as a string value</param>
    /// <param name="orderSide">Order side</param>
    /// <param name="orderStatus">Order status</param>
    /// <param name="pageSize">The number of candles to return 5, 25, 50, 100, default value is 25</param>
    /// <param name="pageLink">Get the results for the next or previous page</param>
    public static async Task<BxHttpResponse<List<Order>>> GetOrders(this BxHttpClient httpClient, string symbol = "", string handle = "", OrderSide orderSide = OrderSide.None, OrderStatus orderStatus = OrderStatus.None, int pageSize = 25, BxPageLinks.PageLink? pageLink = null)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.Orders)
            .AddQueryParam("symbol", symbol)
            .AddQueryParam("handle", handle)
            .AddQueryParam("side", orderSide)
            .AddQueryParam("status", orderStatus)
            .AddPagination(pageSize, useMetaData: true)
            .AddPageLink(pageLink ?? BxPageLinks.PageLink.Empty)
            .Build();

        return await httpClient.Get<List<Order>>(bxPath);
    }

    /// <summary>
    /// Gets an order by ID
    /// </summary>
    /// <param name="orderId">The order ID</param>
    /// <param name="tradingAccountId"></param>
    public static async Task<BxHttpResponse<Order>> GetOrder(this BxHttpClient httpClient, string orderId, string tradingAccountId)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.OrdersOrderId)
            .AddResourceId(orderId)
            .AddQueryParam("tradingAccountId", tradingAccountId)
            .Build();

        return await httpClient.Get<Order>(bxPath);
    }

    public static async Task<BxHttpResponse<CreateOrder>> CreateOrder(this BxHttpClient httpClient, string tradingAccountId, string symbol, OrderType type, OrderSide side, decimal quantity, decimal price = 0M, decimal stopPrice = 0M, OrderTimeInForce timeInForce = OrderTimeInForce.None, string clientOrderId = "", bool allowBorrow = false)
    {
        // Do some validation
        var priceNormalized = await httpClient.FormatValue(PrecisionType.QuotePrecision, symbol, price);
        var stopPriceNormalized = await httpClient.FormatValue(PrecisionType.QuotePrecision, symbol, stopPrice);
        var quantityNormalized = await httpClient.FormatValue(PrecisionType.BasePrecision, symbol, quantity);

        var bxPath = new EndpointPathBuilder(BxApiEndpoint.Orders)
            .Build();

        var command = new CreateOrderCommand("V3CreateOrder",
            ClientOrderId: string.IsNullOrWhiteSpace(clientOrderId) ? null : clientOrderId,
            Symbol: symbol,
            Type: type.ToString().ToUpperInvariant(), 
            Side: side.ToString().ToUpperInvariant(),
            Price: price == 0 ? null : priceNormalized.ToString(CultureInfo.InvariantCulture),
            StopPrice: stopPrice == 0 ? null : stopPriceNormalized.ToString(CultureInfo.InvariantCulture),
            Quantity: quantityNormalized.ToString(CultureInfo.InvariantCulture),
            TimeInForce: timeInForce == OrderTimeInForce.None ? null : timeInForce.ToString().ToUpperInvariant(),
            AllowBorrow: allowBorrow,
            TradingAccountId: tradingAccountId);

        return await httpClient.Post<CreateOrder, CreateOrderCommand>(bxPath, command);
    }

    public static async Task<BxHttpResponse<CancelAllOrders>> CancelAllOpenOrders(this BxHttpClient httpClient, string tradingAccountId)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.Command)
            .Build();

        var command = new CancelAllOrdersCommand("V1CancelAllOrders", 
            TradingAccountId: tradingAccountId);

        return await httpClient.Post<CancelAllOrders, CancelAllOrdersCommand>(bxPath, command);
    }

    public static async Task<BxHttpResponse<CancelAllOrders>> CancelAllOpenOrdersByMarket(this BxHttpClient httpClient, string tradingAccountId, string symbol)
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.Command)
            .Build();

        var command = new CancelAllOrdersByMarketCommand("V1CancelAllOrdersByMarket", 
            Symbol: symbol, 
            TradingAccountId: tradingAccountId);

        return await httpClient.Post<CancelAllOrders, CancelAllOrdersByMarketCommand>(bxPath, command);
    }

    public static async Task<BxHttpResponse<CancelOrder>> CancelOrder(this BxHttpClient httpClient, string tradingAccountId, string symbol, string orderId = "", string clientOrderId = "")
    {
        var bxPath = new EndpointPathBuilder(BxApiEndpoint.Command)
            .Build();

        var command = new CancelOrderCommand("V3CancelOrder",
            OrderId: string.IsNullOrWhiteSpace(orderId) ? null : orderId,
            ClientOrderId: string.IsNullOrWhiteSpace(clientOrderId) ? null : clientOrderId,
            Symbol: symbol,
            TradingAccountId: tradingAccountId);

        return await httpClient.Post<CancelOrder, CancelOrderCommand>(bxPath, command);
    }

    // TODO: Cancel All Open Limit Orders after Delay
    // POST /command?commandType=V1DelayedCancelAllOrders

    // TODO: Cancel All Open Limit Orders after Delay Disable
    // POST /command?commandType=V1UnsetDelayedCancelAllOrders
}
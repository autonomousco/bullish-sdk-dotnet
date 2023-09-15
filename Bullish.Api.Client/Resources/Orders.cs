using System.Globalization;
using Bullish.Api.Client.BxClient;

namespace Bullish.Api.Client.Resources;

public static class Orders
{
    /// <summary>
    /// Gets the orders list based on specified filters.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="handle">Unique numeric identifier generated on the client side expressed as a string value</param>
    /// <param name="orderSide">Order side</param>
    /// <param name="orderStatus">Order status</param>
    /// <param name="pageSize">The number of candles to return 5, 25, 50, 100, default value is 25</param>
    /// <param name="pageLink">Get the results for the next or previous page</param>
    public static async Task<BxHttpResponse<List<Order>>> GetOrders(this BxHttpClient httpClient, string symbol = "", string handle = "", OrderSide orderSide = OrderSide.None, OrderStatus orderStatus = OrderStatus.None, int pageSize = 25, BxPageLink? pageLink = null)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.Orders)
            .AddQueryParam("symbol", symbol)
            .AddQueryParam("handle", handle)
            .AddQueryParam("side", orderSide)
            .AddQueryParam("status", orderStatus)
            .AddPagination(pageSize, useMetaData: true)
            .AddPageLink(pageLink ?? BxPageLink.Empty)
            .Build();

        return await httpClient.Get<List<Order>>(bxPath);
    }

    /// <summary>
    /// Gets an order by ID
    /// </summary>
    /// <param name="orderId">The order ID</param>
    public static async Task<BxHttpResponse<Order>> GetOrder(this BxHttpClient httpClient, string orderId)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.OrdersOrderId)
            .AddResourceId(orderId)
            .Build();

        return await httpClient.Get<Order>(bxPath);
    }

    public static async Task<BxHttpResponse<CreateOrderResponse>> CreateOrder(this BxHttpClient httpClient, string symbol, OrderType type, OrderSide side, decimal quantity, decimal price = 0M, decimal stopPrice = 0M, OrderTimeInForce timeInForce = OrderTimeInForce.None, string handle = "", bool allowMargin = false, bool isTest = false)
    {
        // Do some validation
        var priceNormalized = await httpClient.FormatValue(PrecisionType.QuotePrecision, symbol, price);
        var stopPriceNormalized = await httpClient.FormatValue(PrecisionType.QuotePrecision, symbol, stopPrice);
        var quantityNormalized = await httpClient.FormatValue(PrecisionType.BasePrecision, symbol, quantity);

        var bxPath = new BxPathBuilder(BxApiEndpoint.Orders)
            .AddQueryParam("test", isTest)
            .Build();

        // TODO: this will not work. Nonce has not yet been configured via auto-login
        var commandRequest = httpClient.GetCommandRequest();

        var request = new CreateOrderRequest
        {
            Timestamp = commandRequest.Timestamp,
            Nonce = commandRequest.Nonce,
            Authorizer = commandRequest.Authorizer,
            Command = new CreateOrderCommand
            {
                Handle = string.IsNullOrWhiteSpace(handle) ? null : handle,
                Symbol = symbol,
                Type = type.ToString().ToUpperInvariant(),
                Side = side.ToString().ToUpperInvariant(),
                Price = price == 0 ? null : priceNormalized.ToString(CultureInfo.InvariantCulture),
                StopPrice = stopPrice == 0 ? null : stopPriceNormalized.ToString(CultureInfo.InvariantCulture),
                Quantity = quantityNormalized.ToString(CultureInfo.InvariantCulture),
                TimeInForce = timeInForce == OrderTimeInForce.None ? null : timeInForce.ToString().ToUpperInvariant(),
                AllowMargin = allowMargin,
            }
        };

        return await httpClient.Post<CreateOrderResponse, CreateOrderRequest>(bxPath, request);
    }

    public static async Task<BxHttpResponse<CancelAllOrdersResponse>> CancelAllOpenOrders(this BxHttpClient httpClient, string tradingAccountId)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.CommandCancelAllOpenOrders)
            .Build();

        var commandRequest = httpClient.GetCommandRequest();

        var request = new CancelAllOrdersRequest
        {
            Timestamp = commandRequest.Timestamp,
            Nonce = commandRequest.Nonce,
            Authorizer = commandRequest.Authorizer,
            Command = new CancelAllOrdersCommand
            {
                TradingAccountId = tradingAccountId,
            }
        };

        return await httpClient.Post<CancelAllOrdersResponse, CancelAllOrdersRequest>(bxPath, request);
    }

    public static async Task<BxHttpResponse<CancelAllOrdersResponse>> CancelAllOpenOrdersByMarket(this BxHttpClient httpClient, string tradingAccountId, string symbol)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.CommandCancelAllOpenOrders)
            .Build();

        var commandRequest = httpClient.GetCommandRequest();

        var request = new CancelAllOrdersByMarketRequest
        {
            Timestamp = commandRequest.Timestamp,
            Nonce = commandRequest.Nonce,
            Authorizer = commandRequest.Authorizer,
            Command = new CancelAllOrdersCommandByMarket
            {
                Symbol = symbol,
                TradingAccountId = tradingAccountId,
            }
        };

        return await httpClient.Post<CancelAllOrdersResponse, CancelAllOrdersByMarketRequest>(bxPath, request);
    }
}
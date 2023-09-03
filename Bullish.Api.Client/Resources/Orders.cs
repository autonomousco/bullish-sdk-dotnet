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
    public static async Task<BxHttpResponse<List<Order>>> GetOrders(this BxHttpClient httpClient, string symbol = "", string handle = "", OrderSide orderSide = OrderSide.None, OrderStatus orderStatus = OrderStatus.None)
    {
        var bxPath = new BxPathBuilder(BxApiEndpoint.Orders)
            .AddQueryParam("symbol", symbol)
            .AddQueryParam("handle", handle)
            .AddQueryParam("side", orderSide)
            .AddQueryParam("status", orderStatus)
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
            .AddQueryParam("orderId", orderId)
            .Build();

        return await httpClient.Get<Order>(bxPath);
    }
}
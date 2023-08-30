namespace Bullish.Api.Client.Resources;

public static class Orders
{
    // Get Orders
    // /orders
    public static async Task<BxHttpResponse<List<Order>>> GetOrders(this BxHttpClient httpClient, string symbol = "", string handle = "", OrderSide orderSide = OrderSide.None, OrderStatus orderStatus = OrderStatus.None)
    {
        var pathBuilder = new BxPathBuilder(BxApiEndpoint.Orders)
            .AddQueryParam("symbol", symbol)
            .AddQueryParam("handle", handle)
            .AddQueryParam("side", orderSide)
            .AddQueryParam("status", orderStatus);

        return await httpClient.MakeRequest<List<Order>>(pathBuilder.Path);
    }
}
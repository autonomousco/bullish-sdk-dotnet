namespace Bullish.Internals;

internal interface ICommand { }

internal record Command(string CommandType, string Timestamp, string Nonce, string Authorizer) 
{
    protected Command(string commandType) : this(commandType, string.Empty, string.Empty, string.Empty) { }
}

internal sealed record CancelAllOrdersCommand(string TradingAccountId) : Command("V1CancelAllOrders"), ICommand;

internal sealed record CancelAllOrdersByMarketCommand(string Symbol, string TradingAccountId) : Command("V1CancelAllOrdersByMarket"), ICommand;

internal sealed record CreateOrderCommand(string? Handle, string Symbol, string Type, string Side, string? Price, string? StopPrice, string Quantity, string? TimeInForce, bool AllowMargin) : Command("V1CreateOrder"), ICommand;

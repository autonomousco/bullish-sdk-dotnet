namespace Bullish.Internals;

internal interface ICommand
{
    public string CommandType { get; }
}

internal sealed record CancelAllOrdersCommand(string CommandType, string TradingAccountId) : ICommand { }

internal sealed record CancelAllOrdersByMarketCommand(string CommandType, string Symbol, string TradingAccountId) : ICommand { }

internal sealed record CreateOrderCommand(string CommandType, string? Handle, string Symbol, string Type, string Side, string? Price, string? StopPrice, string Quantity, string? TimeInForce, bool AllowMargin) : ICommand { }

internal sealed record CancelOrderCommand(string CommandType, string? OrderId, string? Handle, string Symbol, string TradingAccountId) : ICommand { }
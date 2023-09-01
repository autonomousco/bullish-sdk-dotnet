namespace Bullish.Api.Client.HttpClient;

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
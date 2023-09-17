using System.Net;

namespace Bullish;

public record BxHttpError
{
    public required int ErrorCode { get; init; }
    public required string ErrorCodeName { get; init; }
    public required string Message { get; init; }
    public required object Raw { get; init; }

    // TODO: Update various constructors to Record style :this() { }
    public static BxHttpError Empty => new()
    {
        ErrorCode = 0,
        ErrorCodeName = string.Empty,
        Message = string.Empty,
        Raw = new object(),
    };

    public static BxHttpError Error(string message) => new()
    {
        Message = message,
        Raw = new object(),
        ErrorCode = 0,
        ErrorCodeName = string.Empty,
    };
    
    public static BxHttpError Error(HttpStatusCode httpStatusCode, string message) => new()
    {
        Message = message,
        Raw = new object(),
        ErrorCode = (int)httpStatusCode,
        ErrorCodeName = "HttpStatusCode",
    };
}
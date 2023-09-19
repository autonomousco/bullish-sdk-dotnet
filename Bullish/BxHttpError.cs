using System.Net;

namespace Bullish;

public sealed record BxHttpError(int ErrorCode, string ErrorCodeName, string Message, object Raw)
{
    public BxHttpError() : this(0, string.Empty, string.Empty, new object()) { }

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
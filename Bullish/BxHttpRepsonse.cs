namespace Bullish;

public sealed record BxHttpResponse<T>
{
    public required bool IsSuccess { get; init; }
    public required T? Result { get; init; }
    public required BxHttpError Error { get; init; }
    public required BxPageLinks PageLinks { get; init; }

    public static BxHttpResponse<T> Success(T result, BxPageLinks pageLinks) => new()
    {
        Result = result,
        IsSuccess = true,
        Error = BxHttpError.Empty,
        PageLinks = pageLinks,
    };
    
    public static BxHttpResponse<T> Success(T result) => new()
    {
        Result = result,
        IsSuccess = true,
        Error = BxHttpError.Empty,
        PageLinks = BxPageLinks.Empty,
    };
    
    public static BxHttpResponse<T> Success() => new()
    {
        Result = default,
        IsSuccess = true,
        Error = BxHttpError.Empty,       
        PageLinks = BxPageLinks.Empty,
    };

    public static BxHttpResponse<T> Failure(BxHttpError error) => new()
    {
        Result = default,
        IsSuccess = false,
        Error = error,
        PageLinks = BxPageLinks.Empty,
    };

    public static BxHttpResponse<T> Failure(string message) => new()
    {
        Result = default,
        IsSuccess = false,
        Error = BxHttpError.Error(message),
        PageLinks = BxPageLinks.Empty,
    };
}
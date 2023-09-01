namespace Bullish.Api.Client.HttpClient;

public record BxHttpResponse<T>
{
    public required bool IsSuccess { get; init; }
    public required T? Result { get; init; }
    public required BxHttpError Error { get; init; }

    public static BxHttpResponse<T> Success(T result) => new()
    {
        Result = result,
        IsSuccess = true,
        Error = BxHttpError.Empty
    };

    public static BxHttpResponse<T> Failure(BxHttpError error) => new()
    {
        Result = default,
        IsSuccess = false,
        Error = error
    };

    public static BxHttpResponse<T> Failure(string message) => new()
    {
        Result = default,
        IsSuccess = false,
        Error = BxHttpError.Error(message)
    };
}
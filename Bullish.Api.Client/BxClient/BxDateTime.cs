namespace Bullish.Api.Client.BxClient;

public record BxDateTime
{
    private readonly Type _type;
    private readonly DateTime _timestamp;

    private enum Type
    {
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
    }

    private BxDateTime(Type type, DateTime timestamp)
    {
        if (timestamp.Kind != DateTimeKind.Utc)
            throw new Exception("Datetime must be of kind UTC.");

        _timestamp = timestamp;
        _type = type;
    }

    public static BxDateTime GreaterThan(DateTime timestamp) => new(Type.GreaterThan, timestamp);

    public static BxDateTime GreaterThanOrEqual(DateTime timestamp) => new(Type.GreaterThanOrEqual, timestamp);

    public static BxDateTime LessThan(DateTime timestamp) => new(Type.LessThan, timestamp);

    public static BxDateTime LessThanOrEqual(DateTime timestamp) => new(Type.LessThanOrEqual, timestamp);

    public (string Name, string Value) AsQueryParam()
    {
        return _type switch
        {
            Type.GreaterThan => ("createdAtDatetime[gt]", $"{_timestamp}"),
            Type.GreaterThanOrEqual => ("createdAtDatetime[gte]", $"{_timestamp}"),
            Type.LessThan => ("createdAtDatetime[lt]", $"{_timestamp}"),
            Type.LessThanOrEqual => ("createdAtDatetime[lte]", $"{_timestamp}"),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
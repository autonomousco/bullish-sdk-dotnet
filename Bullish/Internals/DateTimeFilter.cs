namespace Bullish.Internals;

internal sealed record DateTimeFilter
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

    private DateTimeFilter(Type type, DateTime timestamp)
    {
        if (timestamp.Kind != DateTimeKind.Utc)
            throw new Exception("Datetime must be of kind UTC.");

        _timestamp = timestamp;
        _type = type;
    }

    public static DateTimeFilter GreaterThan(DateTime timestamp) => new(Type.GreaterThan, timestamp);

    public static DateTimeFilter GreaterThanOrEqual(DateTime timestamp) => new(Type.GreaterThanOrEqual, timestamp);

    public static DateTimeFilter LessThan(DateTime timestamp) => new(Type.LessThan, timestamp);

    public static DateTimeFilter LessThanOrEqual(DateTime timestamp) => new(Type.LessThanOrEqual, timestamp);

    public (string Name, string Value) AsQueryParam()
    {
        return _type switch
        {
            Type.GreaterThan => ("createdAtDatetime[gt]", _timestamp.AsBxDateTime()),
            Type.GreaterThanOrEqual => ("createdAtDatetime[gte]", _timestamp.AsBxDateTime()),
            Type.LessThan => ("createdAtDatetime[lt]", _timestamp.AsBxDateTime()),
            Type.LessThanOrEqual => ("createdAtDatetime[lte]", _timestamp.AsBxDateTime()),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
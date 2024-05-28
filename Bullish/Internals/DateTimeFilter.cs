namespace Bullish.Internals;

internal sealed record DateTimeFilter
{
    private const string DefaultKeyword = "createdAtDatetime";

    private readonly Type _type;
    private readonly DateTime _timestamp;
    private readonly string _keyword;

    private enum Type
    {
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
    }

    private DateTimeFilter(Type type, DateTime timestamp, string keyword = "")
    {
        if (timestamp.Kind != DateTimeKind.Utc)
            throw new Exception("Datetime must be of kind UTC.");

        _keyword = string.IsNullOrWhiteSpace(keyword) ? DefaultKeyword : keyword;
        _timestamp = timestamp;
        _type = type;
    }

    public static DateTimeFilter GreaterThan(DateTime timestamp, string keyword = "") => new(Type.GreaterThan, timestamp, keyword);

    public static DateTimeFilter GreaterThanOrEqual(DateTime timestamp, string keyword = "") => new(Type.GreaterThanOrEqual, timestamp, keyword);

    public static DateTimeFilter LessThan(DateTime timestamp, string keyword = "") => new(Type.LessThan, timestamp, keyword);

    public static DateTimeFilter LessThanOrEqual(DateTime timestamp, string keyword = "") => new(Type.LessThanOrEqual, timestamp, keyword);

    public (string Name, string Value) AsQueryParam()
    {
        return _type switch
        {
            Type.GreaterThan => ($"{_keyword}[gt]", _timestamp.AsBxDateTime()),
            Type.GreaterThanOrEqual => ($"{_keyword}[gte]", _timestamp.AsBxDateTime()),
            Type.LessThan => ($"{_keyword}[lt]", _timestamp.AsBxDateTime()),
            Type.LessThanOrEqual => ($"{_keyword}[lte]", _timestamp.AsBxDateTime()),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
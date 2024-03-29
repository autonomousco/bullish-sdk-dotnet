namespace Bullish.Internals;

[Obsolete("No longer used internally. Awaiting upgrade to thread safe stateful nonce.")]
internal sealed record BxNonce
{
    public long LowerBound { get; init; }
    public long UpperBound { get; init; }

    public long Value { get; private set; } = -1;

    public static BxNonce Empty => new()
    {
        UpperBound = 0,
        LowerBound = 0,
    };

    public long NextValue()
    {
        if (Value == -1)
        {
            Value = DateTime.UtcNow.ToUnixTimeMicroseconds();
            return Value;
        }

        if (Value == UpperBound)
            throw new Exception("Value cannot exceed upper bounds");

        return ++Value;
    }

    public bool IsValid()
    {
        if (LowerBound == 0 || UpperBound == 0)
            return false;

        var todayUtc = DateTime.UtcNow.TodayUtc();

        var localLower = todayUtc.ToUnixTimeMicroseconds();
        var localUpper = localLower + 86399999000; // Add time up to 1ms before midnight

        return LowerBound == localLower && UpperBound == localUpper;
    }
}
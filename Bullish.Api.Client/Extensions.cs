namespace Bullish.Api.Client;

public static class Extensions
{
    public static DateTime TodayUtc(this DateTime dateTime)
    {
        if (dateTime.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Must be DateTimeKind.Utc", nameof(dateTime));
        
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0,0,0, DateTimeKind.Utc);
    }

    public static long ToUnixTimeMicroseconds(this DateTime dateTime)
    {
        if (dateTime.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Must be DateTimeKind.Utc", nameof(dateTime));
        
        return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds() * 1000;
    }

 
}
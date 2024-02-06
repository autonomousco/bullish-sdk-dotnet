using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bullish.Internals;

namespace Bullish;

internal static class Extensions
{
    public static int GetDecimalPlaces(this decimal n)
    {
        n = Math.Abs(n); // Make sure it's positive
        n -= (int)n; // Remove the integer part
            
        var decimalPlaces = 0;
            
        while (n > 0)
        {
            decimalPlaces++;
            n *= 10;
            n -= (int)n;
        }

        return decimalPlaces;
    }
    
    public static DateTime TodayUtc(this DateTime dateTime)
    {
        if (dateTime.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Must be DateTimeKind.Utc", nameof(dateTime));
        
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0,0,0, DateTimeKind.Utc);
    }

    public static string AsBxDateTime(this DateTime dateTime)
    {
        var iso8601 = $"{dateTime:O}";

        // Truncate the extra precision 
        return $"{iso8601[..^5]}Z";
    }

    public static string ToBxTimeBucket(this TimeBucket timeBucket)
    {
        return timeBucket switch
        {
            TimeBucket.OneMinute => "1m",
            TimeBucket.FiveMinutes => "5m",
            TimeBucket.ThirtyMinutes => "30m",
            TimeBucket.OneHour => "1h",
            TimeBucket.SixHours => "6h",
            TimeBucket.TwelveHours => "12h",
            TimeBucket.OneDay => "1d",
            _ => throw new ArgumentOutOfRangeException(nameof(timeBucket), timeBucket, null)
        };
    }

    public static long ToUnixTimeMicroseconds(this DateTime dateTime)
    {
        if (dateTime.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Must be DateTimeKind.Utc", nameof(dateTime));
        
        return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds() * 1000;
    }
    
    public static long ToUnixTimeMilliseconds(this DateTime dateTime)
    {
        if (dateTime.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Must be DateTimeKind.Utc", nameof(dateTime));
        
        return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
    }
    
    private static JsonSerializerOptions GetJsonSerializerOptions(bool writeIndented = false)
    {
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            WriteIndented = writeIndented,
            Converters = { new JsonConverters.BoolConverter() },
        };

        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        return jsonSerializerOptions;
    }

    public static string Serialize(object obj, bool writeIndented = false)
    {
        return JsonSerializer.Serialize(obj, GetJsonSerializerOptions(writeIndented));
    }

    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, GetJsonSerializerOptions());
    }

    public static T? DeserializeBase64<T>(string base64String)
    {
        var json = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));
        return JsonSerializer.Deserialize<T>(json, GetJsonSerializerOptions());
    }
}
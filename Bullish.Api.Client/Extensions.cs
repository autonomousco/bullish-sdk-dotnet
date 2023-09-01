using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bullish.Api.Client;

internal static class Extensions
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
    
    private static JsonSerializerOptions GetJsonSerializerOptions(bool writeIndented = false)
    {
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            WriteIndented = writeIndented,
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
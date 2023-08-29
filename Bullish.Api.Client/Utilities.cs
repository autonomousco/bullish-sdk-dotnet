using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

[assembly:InternalsVisibleTo("ApiTester")]

namespace Bullish.Api.Client;

internal static class Utilities
{
    private static JsonSerializerOptions GetJsonSerializerOptions(bool writeIndented = false)
    {
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
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
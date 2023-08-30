namespace Bullish.Api.Client;

public class BxPathBuilder
{
    private readonly List<string> _components = new();

    public string Path => string.Concat(_components);

    public BxPathBuilder(BxApiEndpoint endpoint, string slug = "")
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            _components.Add(Data.BxApiEndpoints[endpoint]);
            return;
        }

        var components = Data.BxApiEndpoints[endpoint].Split('{', '}');
        components[1] = slug;

        _components.Add(string.Concat(components));
    }

    public BxPathBuilder AddQueryParam<T>(string name, T value)
    {
        var value2 = string.Empty;
        if (typeof(T).IsEnum)
        {
            value2 = Convert.ToInt32(value) == 0 ? string.Empty : value.ToString().ToUpperInvariant();
        }

        return AddQueryParam(name, value2);;
    }

    public BxPathBuilder AddQueryParam(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return this;

        var prefix = _components.Any(i => i.StartsWith("?")) ? "&" : "?";

        _components.Add($"{prefix}{name}={value.ToUpperInvariant()}");

        return this;
    }
}
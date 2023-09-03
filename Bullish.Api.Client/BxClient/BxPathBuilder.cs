namespace Bullish.Api.Client.BxClient;


public class BxPathBuilder
{
    private readonly BxApiEndpoint _endpoint;

    private readonly BxEndpoint _bxEndpoint;
    
    private readonly List<string> _components = new();

    public BxPathBuilder(BxApiEndpoint endpoint)
    {
        _endpoint = endpoint;
       
        _bxEndpoint = Data.BxApiEndpoints[endpoint];
       
        _components.Add(_bxEndpoint.Version);
        
        _components.Add(_bxEndpoint.Path);
    }

    public BxPathBuilder AddResourceId(string resourceId)
    {
        var components = _components[1].Split('{', '}');
        components[1] = resourceId;

        _components[1] = string.Concat(components);

        return this;
    }

    public BxPath Build()
    {
        return new BxPath(_endpoint, string.Concat(_components), _bxEndpoint.UseAuth);
    }

    public BxPathBuilder AddQueryParam<T>(string name, T value)
    {
        if (value is null)
            throw new Exception($"Value for {name} cannot be null.");

        var valueStr = value.ToString();
        
        if(string.IsNullOrWhiteSpace(valueStr))
            throw new Exception($"Value string for {name} cannot be null.");

        var value2 = string.Empty;
        if (typeof(T).IsEnum)
        {
            value2 = Convert.ToInt32(value) == 0 ? string.Empty : valueStr.ToUpperInvariant();
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

    public BxPathBuilder AddQueryParam(BxDateTime timestamp)
    {
        var (name, value) = timestamp.AsQueryParam();
        return AddQueryParam(name, value);
    }
}
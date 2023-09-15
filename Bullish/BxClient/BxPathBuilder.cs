namespace Bullish.BxClient;


public class BxPathBuilder
{
    private readonly BxApiEndpoint _endpoint;

    private readonly BxEndpoint _bxEndpoint;
    
    private readonly List<string> _components = new();

    private bool _usePagination = false;

    public BxPathBuilder(BxApiEndpoint endpoint)
    {
        _endpoint = endpoint;
       
        _bxEndpoint = Constants.BxApiEndpoints[endpoint];
       
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
        return new BxPath(_endpoint, string.Concat(_components), _bxEndpoint.UseAuth, _usePagination);
    }

    public BxPathBuilder AddPageLink(BxPageLink pageLink)
    {
        AddQueryParam(pageLink.Name, pageLink.Value);

        return this;
    }

    public BxPathBuilder AddPagination(int pageSize, bool useMetaData)
    {
        AddQueryParam("_pageSize", pageSize.ToString());
        AddQueryParam("_metaData", useMetaData);

        _usePagination = true;

        return this;
    }

    public BxPathBuilder AddQueryParam<T>(string name, T value)
    {
        if (value is null)
            throw new Exception($"Value for {name} cannot be null.");

        var valueStr = value.ToString();
        
        if(string.IsNullOrWhiteSpace(valueStr))
            throw new Exception($"Value string for {name} cannot be null.");

        var value2 = typeof(T).IsEnum switch
        {
            true when value is TimeBucket bucket => bucket.ToBxTimeBucket(),
            true => Convert.ToInt32(value) == 0 ? string.Empty : valueStr.ToUpperInvariant(),
            false when value is bool => valueStr.ToLowerInvariant(),
            false => valueStr,
        };

        return AddQueryParam(name, value2);;
    }

    public BxPathBuilder AddQueryParam(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return this;

        var prefix = _components.Any(i => i.Contains('?')) ? "&" : "?";

        _components.Add($"{prefix}{name}={value}");

        return this;
    }

    public BxPathBuilder AddQueryParam(BxDateTime timestamp)
    {
        var (name, value) = timestamp.AsQueryParam();
        return AddQueryParam(name, value);
    }
}
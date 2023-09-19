namespace Bullish.Internals;

internal sealed class EndpointPathBuilder
{
    private readonly Endpoint _endpoint;
    
    private readonly List<string> _components = new();

    private bool _usePagination = false;

    public EndpointPathBuilder(BxApiEndpoint endpoint)
    {
        _endpoint = Constants.BxApiEndpoints[endpoint];
       
        _components.Add(_endpoint.Version);
        
        _components.Add(_endpoint.Path);
    }

    public EndpointPathBuilder AddResourceId(string resourceId)
    {
        var components = _components[1].Split('{', '}');
        components[1] = resourceId;

        _components[1] = string.Concat(components);

        return this;
    }

    public EndpointPath Build()
    {
        return new EndpointPath(string.Concat(_components), _endpoint.UseAuth, _usePagination);
    }

    public EndpointPathBuilder AddPageLink(BxPageLinks.PageLink pageLink)
    {
        AddQueryParam(pageLink.Name, pageLink.Value);

        return this;
    }

    public EndpointPathBuilder AddPagination(int pageSize, bool useMetaData)
    {
        AddQueryParam("_pageSize", pageSize.ToString());
        AddQueryParam("_metaData", useMetaData);

        _usePagination = true;

        return this;
    }

    public EndpointPathBuilder AddQueryParam<T>(string name, T value)
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

    public EndpointPathBuilder AddQueryParam(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return this;

        var prefix = _components.Any(i => i.Contains('?')) ? "&" : "?";

        _components.Add($"{prefix}{name}={value}");

        return this;
    }

    public EndpointPathBuilder AddQueryParam(DateTimeFilter timestamp)
    {
        var (name, value) = timestamp.AsQueryParam();
        return AddQueryParam(name, value);
    }
}
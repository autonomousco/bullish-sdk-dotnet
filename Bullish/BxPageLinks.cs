namespace Bullish;

public sealed record BxPageLinks(string Next, string Previous)
{
    public static BxPageLinks Empty => new(string.Empty, string.Empty);

    public PageLink NextPage => new("_nextPage", GetPageHash(Next));

    public PageLink PrevPage => new("_previousPage", GetPageHash(Previous));

    private static string GetPageHash(string url)
    {
        var index = url.LastIndexOf("=", StringComparison.Ordinal) + 1;
        return url[index..];
    }
    
    public sealed record PageLink(string Name, string Value)
    {
        public static PageLink Empty => new(string.Empty, string.Empty);
    }
}
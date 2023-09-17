namespace Bullish;

public sealed record BxPageLinks(string Next, string Previous)
{
    public static BxPageLinks Empty => new(string.Empty, string.Empty);

    public BxPageLink NextPage => new("_nextPage", GetPageHash(Next));

    public BxPageLink PrevPage => new("_previousPage", GetPageHash(Previous));

    private static string GetPageHash(string url)
    {
        var index = url.LastIndexOf("=", StringComparison.Ordinal) + 1;
        return url[index..];
    }
}
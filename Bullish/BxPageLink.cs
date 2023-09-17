namespace Bullish;

public sealed record BxPageLink(string Name, string Value)
{
    public static BxPageLink Empty => new(string.Empty, string.Empty);
}
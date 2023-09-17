namespace Bullish;

internal sealed record BxPath(BxApiEndpoint Endpoint, string Path, bool UseAuth, bool UsePagination);
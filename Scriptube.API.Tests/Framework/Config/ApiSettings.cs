namespace Scriptube.API.Tests.Framework.Config;

public sealed record ApiSettings
{
    public required string BaseUrl { get; init; }
    public int TimeoutMs { get; init; } = 30_000;
    public int RetryCount { get; init; } = 2;
    public int RetryDelayMs { get; init; } = 500;
    public required string ApiKey { get; init; }
    public int MinVideoCostCredits { get; init; } = 4;
}
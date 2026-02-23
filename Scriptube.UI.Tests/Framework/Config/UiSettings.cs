namespace Scriptube.UI.Tests.Framework.Config;

public sealed class UiSettings
{
    public required string BaseUrl { get; init; }
    public bool Headless { get; init; } = true;
    public int TimeoutMs { get; init; } = 30_000;
    public int SlowMoMs { get; init; } = 0;
    public required AuthSettings Auth { get; init; }
}

public sealed class AuthSettings
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
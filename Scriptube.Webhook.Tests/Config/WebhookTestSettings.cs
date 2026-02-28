namespace Scriptube.Webhook.Tests.Config;

public sealed class WebhookTestSettings
{
    public string BaseUrl { get; init; } = "";
    public string AdminToken { get; init; } = "";
    public int PollTimeoutMs { get; init; } = 60_000;
    public int PollIntervalMs { get; init; } = 1_000;
}
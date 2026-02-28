using NUnit.Framework;
using Scriptube.Webhook.Tests.Builders;
using Scriptube.Webhook.Tests.Clients;
using Scriptube.Webhook.Tests.TestBase;
using Scriptube.Webhook.Tests.Models;

namespace Scriptube.Webhook.Tests.Fixtures;

public sealed class WebhookRegistrationFixture
{
    public ScriptubeWebhooksClient Webhooks { get; }
    public string WebhookId { get; private set; } = "";
    public string? WebhookSecret { get; private set; }

    public WebhookRegistrationFixture()
    {
        var s = TestConfiguration.Scriptube;
        if (string.IsNullOrWhiteSpace(s.ApiKey)) throw new InvalidOperationException("Secrets:ApiKey is not configured.");
        Webhooks = new ScriptubeWebhooksClient(s.BaseUrl, s.ApiKey);
    }

    public async Task RegisterAsync(string receiverHookUrl, string? secret = null, List<string>? events = null)
    {
    var s = secret;
    if (string.IsNullOrWhiteSpace(s) || s.Length < 16)
        s = "test-secret-000000"; // 16+ chars

    var ev = events;
    if (ev == null || ev.Count == 0)
        ev = new List<string> { "batch.completed" }; // adjust to actual name once confirmed

    var req = new RegisterWebhookRequest
    {
        Url = receiverHookUrl,
        Secret = s,
        Events = ev
    };

    var resp = await Webhooks.RegisterAsync(req);
    WebhookId = resp.Id;
    WebhookSecret = s; // Store the secret used for registration, as Scriptube may not return it in the response.
    }
    public async Task CleanupAsync()
    {
    if (string.IsNullOrWhiteSpace(WebhookId)) return;

    try { await Webhooks.DeleteAsync(WebhookId); }
    catch { /* ignore */ }
    }
}
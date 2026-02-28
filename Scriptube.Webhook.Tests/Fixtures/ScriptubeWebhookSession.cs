using Scriptube.Webhook.Tests.Clients;
using Scriptube.Webhook.Tests.Models;
using Scriptube.Webhook.Tests.TestBase;

using System.Text.Json;

namespace Scriptube.Webhook.Tests.Fixtures;

public static class ScriptubeWebhookSession
{
    private static readonly SemaphoreSlim Gate = new(1, 1);
    private static bool _initialized;

    public static string WebhookId { get; private set; } = "";
    public static string WebhookSecret { get; private set; } = "";
    public static ScriptubeWebhooksClient Webhooks { get; private set; } = null!;

    public static async Task EnsureInitializedAsync()
    {
        if (_initialized) return;

        await Gate.WaitAsync();
        try
        {
            if (_initialized) return;

            var api = TestConfiguration.Scriptube;
            var recv = TestConfiguration.Webhook;

            if (string.IsNullOrWhiteSpace(api.ApiKey))
                throw new InvalidOperationException("Secrets:ApiKey is not configured.");

            if (string.IsNullOrWhiteSpace(recv.BaseUrl))
                throw new InvalidOperationException("Receiver:BaseUrl is not configured.");

            Webhooks = new ScriptubeWebhooksClient(api.BaseUrl, api.ApiKey);

            WebhookSecret = "test-secret-000000"; // >= 16 chars
            var hookUrl = recv.BaseUrl.TrimEnd('/') + "/hook";

            var events = new List<string> { "batch.completed" };

            //  reuse existing webhook for this receiver URL
            var existing = (await Webhooks.ListAsync(ct: default))
                .FirstOrDefault(w =>
                    w.IsActive &&
                    string.Equals(w.Url?.TrimEnd('/'), hookUrl, StringComparison.OrdinalIgnoreCase));

            if (existing != null && !string.IsNullOrWhiteSpace(existing.WebhookId))
            {
                WebhookId = existing.WebhookId;
            }
            else
            {
                var resp = await Webhooks.RegisterAsync(new RegisterWebhookRequest
                {
                    Url = hookUrl,
                    Secret = WebhookSecret,
                    Events = events
                });

                WebhookId = resp.Id;
            }
            if (string.IsNullOrWhiteSpace(WebhookId))
                throw new InvalidOperationException("Webhook session initialized but WebhookId is empty. Initialization did not run or returned the wrong session instance.");
            _initialized = true;
        }
        finally
        {
            Gate.Release();
        }
    }

    // Optional manual cleanup
    public static async Task CleanupAsync()
    {
        if (!_initialized) return;
        if (Webhooks is null) return;
        if (string.IsNullOrWhiteSpace(WebhookId)) return;

        try
        {
            await Webhooks.DeleteAsync(WebhookId);
        }
        catch { }
    }
}
using Scriptube.Webhook.Tests.Models;

namespace Scriptube.Webhook.Tests.Builders;

public sealed class WebhookRegistrationBuilder
{
    private string _url = "";
    private readonly List<string> _events = new();
    private string? _secret;

    public WebhookRegistrationBuilder WithUrl(string url) { _url = url; return this; }
    public WebhookRegistrationBuilder WithEvent(string evt) { _events.Add(evt); return this; }
    public WebhookRegistrationBuilder WithEvents(params string[] evts) { _events.AddRange(evts); return this; }
    public WebhookRegistrationBuilder WithSecret(string secret) { _secret = secret; return this; }

    public RegisterWebhookRequest Build()
    {
        var events = _events.Count == 0
        ? new List<string> { "batch.completed" } // default event
            : new List<string>(_events);

        var secret = string.IsNullOrWhiteSpace(_secret) || _secret.Length < 16
        ? "test-secret-000000" // 16+ chars
        : _secret;

        return new RegisterWebhookRequest
        {
            Url = _url,
            Events = events,
            Secret = secret
        };
    }
}
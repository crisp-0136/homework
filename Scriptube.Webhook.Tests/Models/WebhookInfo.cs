using System.Text.Json.Serialization;

namespace Scriptube.Webhook.Tests.Models;

public sealed class WebhookInfo
{
    [JsonPropertyName("webhook_id")]
    public string WebhookId { get; init; } = "";

    [JsonPropertyName("url")]
    public string Url { get; init; } = "";

    [JsonPropertyName("events")]
    public List<string> Events { get; init; } = new();

    [JsonPropertyName("is_active")]
    public bool IsActive { get; init; }
}
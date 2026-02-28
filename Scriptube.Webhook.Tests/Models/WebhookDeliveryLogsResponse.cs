using System.Text.Json.Serialization;

namespace Scriptube.Webhook.Tests.Models;

public sealed class WebhookDeliveryLogsResponse
{
    [JsonPropertyName("deliveries")]
    public List<WebhookDeliveryLogEntry> Deliveries { get; init; } = new();

    [JsonPropertyName("total")]
    public int Total { get; init; }
}
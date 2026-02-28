using System.Text.Json.Serialization;

namespace Scriptube.Webhook.Tests.Models;

public sealed class WebhookLogEntry
{
    [JsonPropertyName("timestampUtc")]
    public DateTime TimestampUtc { get; init; }

    [JsonPropertyName("eventType")]
    public string? EventType { get; init; }

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; init; }

    [JsonPropertyName("destinationUrl")]
    public string? DestinationUrl { get; init; }
}
using System.Text.Json.Serialization;

namespace Scriptube.Webhook.Tests.Models;

public sealed class WebhookDeliveryLogEntry
{
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; init; } = "";

    [JsonPropertyName("event")]
    public string Event { get; init; } = "";

    [JsonPropertyName("status")]
    public string Status { get; init; } = "";

    [JsonPropertyName("response_code")]
    public int ResponseCode { get; init; }

    [JsonPropertyName("attempts")]
    public int Attempts { get; init; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("next_retry_at")]
    public DateTimeOffset? NextRetryAt { get; init; }
}
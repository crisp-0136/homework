using System.Text.Json;
using System.Text.Json.Serialization;

namespace Scriptube.Webhook.Tests.Models;

public sealed class ReceivedWebhook
{
    [JsonPropertyName("event")]
    public string? Event { get; init; }

    [JsonPropertyName("eventType")]
    public string? EventType { get; init; }

    [JsonPropertyName("type")]
    public string? TypeAlt { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("batch_id")]
    public string? BatchIdSnake { get; init; }

    [JsonPropertyName("batchId")]
    public string? BatchIdCamel { get; init; }

    [JsonPropertyName("data")]
    public JsonElement Data { get; init; }

    [JsonIgnore]
    public string? EffectiveType => EventType ?? Event ?? TypeAlt;

    [JsonIgnore]
    public string? BatchId
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(BatchIdCamel)) return BatchIdCamel;
            if (!string.IsNullOrWhiteSpace(BatchIdSnake)) return BatchIdSnake;

            if (Data.ValueKind == JsonValueKind.Object)
            {
                if (Data.TryGetProperty("batch_id", out var a) && a.ValueKind == JsonValueKind.String) return a.GetString();
                if (Data.TryGetProperty("batchId", out var b) && b.ValueKind == JsonValueKind.String) return b.GetString();
                if (Data.TryGetProperty("id", out var c) && c.ValueKind == JsonValueKind.String) return c.GetString();
            }

            return null;
        }
    }
}
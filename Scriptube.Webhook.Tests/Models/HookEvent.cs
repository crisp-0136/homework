using System.Text.Json.Serialization;

namespace Scriptube.Webhook.Tests.Models;

public sealed class HookEvent
{
    [JsonPropertyName("receivedAtUtc")]
    public DateTime ReceivedAtUtc { get; init; }

    [JsonPropertyName("headers")]
    public Dictionary<string, string> Headers { get; init; } = new(StringComparer.OrdinalIgnoreCase);

    [JsonPropertyName("body")]
    public string Body { get; init; } = "";
}
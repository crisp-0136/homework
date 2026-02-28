using System.Text.Json.Serialization;

namespace Scriptube.Webhook.Tests.Models;

public sealed class AvailableWebhookEvent
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    [JsonPropertyName("description")]
    public string? Description { get; init; }
}
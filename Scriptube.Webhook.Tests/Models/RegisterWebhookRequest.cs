using System.Text.Json.Serialization;

namespace Scriptube.Webhook.Tests.Models;

public sealed class RegisterWebhookRequest
{
    [JsonPropertyName("url")]
    public string Url { get; init; } = "";

    [JsonPropertyName("events")]
    public List<string> Events { get; init; } = new();   // never null

    [JsonPropertyName("secret")]
    public string Secret { get; init; } = "";            // never null
}
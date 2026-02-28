using System.Text.Json.Serialization;

namespace Scriptube.Webhook.Tests.Models;

public sealed class RegisterWebhookResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("url")]
    public string Url { get; init; } = "";

    [JsonPropertyName("secret")]
    public string? Secret { get; init; }
}
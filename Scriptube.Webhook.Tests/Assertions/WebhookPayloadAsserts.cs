using FluentAssertions;

using System.Text.Json;

using Scriptube.Webhook.Tests.Models;

namespace Scriptube.Webhook.Tests.Assertions;

public static class WebhookPayloadAsserts
{
    public static ReceivedWebhook Parse(string rawJson)
    {
        var model = JsonSerializer.Deserialize<ReceivedWebhook>(rawJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        model.Should().NotBeNull("webhook payload must be valid JSON");
        return model!;
    }

    public static void MustHaveCoreFields(ReceivedWebhook payload)
    {
        payload.EffectiveType.Should().NotBeNullOrWhiteSpace();
    }
    public static void MustHaveBatchId(ReceivedWebhook payload)
    {
        payload.BatchId.Should().NotBeNullOrWhiteSpace("payload must include batch id");
    }
}
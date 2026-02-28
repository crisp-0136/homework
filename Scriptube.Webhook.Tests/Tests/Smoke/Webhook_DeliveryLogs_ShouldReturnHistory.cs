using FluentAssertions;
using NUnit.Framework;
using Scriptube.Webhook.Tests.Fixtures;
using Scriptube.Webhook.Tests.TestBase;
using  Allure.NUnit;
namespace Scriptube.Webhook.Tests.Tests.Smoke;

[TestFixture]
[AllureNUnit]
[Category("Webhook")]
[Category("Smoke")]
public sealed class Webhook_DeliveryLogs_ShouldReturnHistory : WebhookTestBase
{
    [SetUp]
    public async Task Setup()
    {
        await ScriptubeWebhookSession.EnsureInitializedAsync();
        await ReceiverAdmin.ClearAsync();
    }

    [Test]
    public async Task Logs()
    {
        var webhooks = ScriptubeWebhookSession.Webhooks;
        var webhookId = ScriptubeWebhookSession.WebhookId;

        webhookId.Should().NotBeNullOrWhiteSpace();

        await webhooks.TestAsync(webhookId);

        _ = await WaitForLastEventAsync();

        // single call, no aggressive polling (avoid 429)
        var logs = await webhooks.GetLogsAsync(webhookId);

        logs.Total.Should().BeGreaterThan(0);
        logs.Deliveries.Should().NotBeNull();
        logs.Deliveries.Count.Should().BeGreaterThan(0);

        logs.Deliveries.Any(d => d.ResponseCode > 0).Should().BeTrue();
        logs.Deliveries.Any(d => !string.IsNullOrWhiteSpace(d.DeliveryId)).Should().BeTrue();
    }
}
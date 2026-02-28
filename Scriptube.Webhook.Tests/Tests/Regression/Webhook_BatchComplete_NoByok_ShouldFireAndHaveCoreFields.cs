using FluentAssertions;
using NUnit.Framework;
using Scriptube.Webhook.Tests.Assertions;
using Scriptube.Webhook.Tests.Clients;
using Scriptube.Webhook.Tests.Fixtures;
using Scriptube.Webhook.Tests.TestBase;
using  Allure.NUnit;
namespace Scriptube.Webhook.Tests.Tests.Regression;

[TestFixture]
[AllureNUnit]
[Category("Webhook")]
[Category("Regression")]
public sealed class Webhook_BatchComplete_NoByok_ShouldFireAndHaveCoreFields : WebhookTestBase
{
    private ScriptubeTranscriptsClient _transcripts = null!;

    [SetUp]
    public async Task Setup()
    {
        await ScriptubeWebhookSession.EnsureInitializedAsync();
        await ReceiverAdmin.ClearAsync();

        var s = TestConfiguration.Scriptube;
        _transcripts = new ScriptubeTranscriptsClient(s.BaseUrl, s.ApiKey);
    }

    [Test]
    public async Task BatchCompleteTriggersWebhook()
    {
        var urls = new List<string>
        {
            "https://www.youtube.com/watch?v=tstENMAN001"
        };

        var batchId = await _transcripts.SubmitAsync(urls, useByok: false);
        batchId.Should().NotBeNullOrWhiteSpace();

        await _transcripts.WaitUntilCompletedAsync(
            batchId,
            TimeSpan.FromMinutes(5),
            TimeSpan.FromSeconds(3));

        // deterministically wait for the batch webhook, not "last event"
        var evt = await WaitForEventAsync(body =>
            body.Contains(batchId, StringComparison.OrdinalIgnoreCase));

        AllureAddText("webhook_raw_body.json", evt.Body, "application/json");

        var payload = WebhookPayloadAsserts.Parse(evt.Body);

        WebhookPayloadAsserts.MustHaveCoreFields(payload);
        WebhookPayloadAsserts.MustHaveBatchId(payload);
        payload.BatchId.Should().Be(batchId);
        payload.EffectiveType.Should().Contain("batch", "this test expects a batch completion webhook");
    }
}
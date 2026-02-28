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
public sealed class Webhook_AvailableEvents_ShouldReturnList : WebhookTestBase
{
    [SetUp]
    public async Task Setup()
    {
        await ScriptubeWebhookSession.EnsureInitializedAsync();
    }

    [Test]
    public async Task AvailableEvents()
    {
        var webhooks = ScriptubeWebhookSession.Webhooks;

        var events = await webhooks.GetAvailableEventsAsync();

        events.Should().NotBeNull();
        events.Count.Should().BeGreaterThan(0);
        events.Any(e => !string.IsNullOrWhiteSpace(e.Name)).Should().BeTrue();
    }
}
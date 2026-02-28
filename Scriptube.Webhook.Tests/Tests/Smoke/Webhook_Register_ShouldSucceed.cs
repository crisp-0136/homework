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
public sealed class Webhook_Register_ShouldSucceed : WebhookTestBase
{
    [SetUp]
    public async Task Setup()
    {
        await ScriptubeWebhookSession.EnsureInitializedAsync();
        await ReceiverAdmin.ClearAsync();
    }

    [Test]
    public Task Register()
    {
        // Under the new approach we register once per run and reuse.
        ScriptubeWebhookSession.WebhookId.Should().NotBeNullOrWhiteSpace("webhook must be registered/reused by ScriptubeWebhookSession");
        ScriptubeWebhookSession.WebhookSecret.Should().NotBeNullOrWhiteSpace();
        ScriptubeWebhookSession.Webhooks.Should().NotBeNull();

        return Task.CompletedTask;
    }
}
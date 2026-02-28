using FluentAssertions;
using NUnit.Framework;
using Scriptube.Webhook.Tests.Fixtures;
using Scriptube.Webhook.Tests.Models;
using Scriptube.Webhook.Tests.TestBase;
using System.Net.Http;
using  Allure.NUnit;
namespace Scriptube.Webhook.Tests.Tests.Regression;

[TestFixture]
[AllureNUnit]
[Category("Webhook")]
[Category("Regression")]
public sealed class Webhook_Register_SsrfTargets_ShouldBeBlocked : WebhookTestBase
{
    [SetUp]
    public async Task Setup()
    {
        await ScriptubeWebhookSession.EnsureInitializedAsync();
    }

    [Test]
    public async Task LocalhostMustBeRejected()
    {
        var webhooks = ScriptubeWebhookSession.Webhooks;

        var req = new RegisterWebhookRequest
        {
            Url = "https://127.0.0.1/hook",
            Secret = "test-secret-000000",                  // >= 16
            Events = new List<string> { "batch.completed" } // required list
        };

        Func<Task> act = async () => await webhooks.RegisterAsync(req);

        var ex = await act.Should().ThrowAsync<HttpRequestException>();

        ex.Which.Message.Should().Contain("400");
        ex.Which.Message.Should().Contain("Invalid webhook URL");
        ex.Which.Message.Should().Contain("Blocked hostname");
    }
}
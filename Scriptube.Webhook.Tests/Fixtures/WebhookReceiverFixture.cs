using NUnit.Framework;
using Scriptube.Webhook.Tests.Clients;
using Scriptube.Webhook.Tests.TestBase;

[SetUpFixture]
public sealed class WebhookReceiverFixture
{
    public static ReceiverAdminClient ReceiverAdmin { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        var s = TestConfiguration.Webhook;

        var api = TestConfiguration.Scriptube;

        if (string.IsNullOrWhiteSpace(api.ApiKey))
            throw new InvalidOperationException("Secrets:ApiKey is not configured.");

        var auth = new ScriptubeAuthClient(api.BaseUrl, api.ApiKey);
        await auth.EnsureApiKeyValidAsync();

        if (string.IsNullOrWhiteSpace(s.BaseUrl))
            throw new InvalidOperationException("Receiver:BaseUrl is not configured.");

        if (string.IsNullOrWhiteSpace(s.AdminToken))
            throw new InvalidOperationException("Receiver:AdminToken is not configured.");

        ReceiverAdmin = new ReceiverAdminClient(s.BaseUrl, s.AdminToken);
        await ReceiverAdmin.ClearAsync();
    }

    [OneTimeTearDown]
    public async Task GlobalTeardown()
    {
        if (ReceiverAdmin != null)
            await ReceiverAdmin.ClearAsync();
    }
}
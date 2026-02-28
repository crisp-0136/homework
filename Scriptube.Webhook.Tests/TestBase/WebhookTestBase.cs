using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using Scriptube.Webhook.Tests.Clients;
using Scriptube.Webhook.Tests.Fixtures;
using Scriptube.Webhook.Tests.Models;
using System.Text;

namespace Scriptube.Webhook.Tests.TestBase;

[AllureSuite("Webhook")]
public abstract class WebhookTestBase
{
    protected ReceiverAdminClient ReceiverAdmin => WebhookReceiverFixture.ReceiverAdmin;

    [SetUp]
    public async Task BaseSetup()
    {
        await ScriptubeWebhookSession.EnsureInitializedAsync();
    }

    protected static string ReceiverHookUrl
    {
        get
        {
            var r = TestConfiguration.Webhook;
            return r.BaseUrl.TrimEnd('/') + "/hook";
        }
    }

    protected async Task<HookEvent> WaitForLastEventAsync(CancellationToken ct = default)
    {
        var s = TestConfiguration.Webhook;
        var timeout = TimeSpan.FromMilliseconds(s.PollTimeoutMs);
        var interval = TimeSpan.FromMilliseconds(s.PollIntervalMs);

        var deadline = DateTimeOffset.UtcNow + timeout;

        while (DateTimeOffset.UtcNow < deadline)
        {
            var last = await ReceiverAdmin.GetLastAsync(ct);
            if (last != null) return last;
            await Task.Delay(interval, ct);
        }

        throw new TimeoutException($"No webhook received within {timeout.TotalSeconds:0}s");
    }

    protected static void AllureAddText(string name, string content, string mime = "application/json")
    {
        var safeName = string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
        if (string.IsNullOrWhiteSpace(safeName)) safeName = "attachment.txt";

        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}_{safeName}");
        File.WriteAllText(filePath, content, Encoding.UTF8);

        AllureApi.AddAttachment(safeName, mime, filePath);
    }
    protected async Task<HookEvent> WaitForEventAsync(
    Func<string, bool> predicate,
    CancellationToken ct = default)
    {
    var s = TestConfiguration.Webhook;
    var timeout = TimeSpan.FromMilliseconds(s.PollTimeoutMs);
    var interval = TimeSpan.FromMilliseconds(s.PollIntervalMs);

    var deadline = DateTimeOffset.UtcNow + timeout;

    while (DateTimeOffset.UtcNow < deadline)
    {
        var events = await ReceiverAdmin.GetAllAsync(ct);

        foreach (var e in events)
        {
            if (!string.IsNullOrWhiteSpace(e.Body) && predicate(e.Body))
                return e;
        }

        await Task.Delay(interval, ct);
    }

    throw new TimeoutException($"Expected webhook event not received within {timeout.TotalSeconds:0}s");
    }
    protected async Task<IReadOnlyList<WebhookLogEntry>> WaitForLogsAsync(
    Func<CancellationToken, Task<IReadOnlyList<WebhookLogEntry>>> fetch,
    Func<IReadOnlyList<WebhookLogEntry>, bool> predicate,
    TimeSpan timeout,
    CancellationToken ct = default)
    {
    var deadline = DateTimeOffset.UtcNow + timeout;

    var delay = TimeSpan.FromSeconds(2);

    while (DateTimeOffset.UtcNow < deadline)
    {
        try
        {
            var logs = await fetch(ct);
            if (predicate(logs)) return logs;

            await Task.Delay(delay, ct);
            continue;
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("429"))
        {
            // exponential-ish backoff, capped
            delay = delay < TimeSpan.FromSeconds(10) ? delay + TimeSpan.FromSeconds(3) : TimeSpan.FromSeconds(10);
            await Task.Delay(delay, ct);
            continue;
        }
    }

    throw new TimeoutException($"Logs did not satisfy condition within {timeout.TotalSeconds:0}s");
    }
}
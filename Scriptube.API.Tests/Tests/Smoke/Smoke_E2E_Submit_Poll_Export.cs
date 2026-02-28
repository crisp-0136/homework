using FluentAssertions;

using NUnit.Framework;

using Scriptube.API.Tests.Framework.Http;
using Scriptube.API.Tests.Framework.Models;

using System.Text.Json;

using Allure.NUnit;
namespace Scriptube.API.Tests.Tests.Smoke;

[TestFixture]
[AllureNUnit]
[Category("API")]
[Category("Smoke")]
public sealed class Smoke_E2E_Submit_Poll_Export : BaseApiTest
{
    [Test]
    public async Task Submit_poll_export_should_work_for_single_video()
    {
        var video = "https://www.youtube.com/watch?v=tstENMAN001";

        // optional: balance before
        var balBefore = await Ctx.Client.GetBalanceAsync();
        balBefore.IsSuccessStatusCode.Should().BeTrue();

        // submit
        var req = new TranscriptRequest
        {
            Urls = new List<string> { video },
            Use_Byok = false,
            Translate_To_English = false
        };

        var submitRes = await Ctx.Client.SubmitTranscriptsAsync(req);
        submitRes.StatusCode.Should().Be(System.Net.HttpStatusCode.Accepted);

        var submit = await submitRes.Content.ReadAsAsync<TranscriptSubmitResponse>();
        submit.Batch_Id.Should().NotBeNullOrWhiteSpace();

        // poll to terminal
        var terminal = await Polling.WaitForBatchTerminalAsync(
            () => Ctx.Client.GetBatchAsync(submit.Batch_Id),
            timeout: TimeSpan.FromMinutes(3),
            interval: TimeSpan.FromSeconds(2));

        var status = terminal.GetProperty("status").GetString() ?? "";
        status.Should().Be("completed");

        // export (formats allowed by spec)
        (await Ctx.Client.ExportAsync(submit.Batch_Id, "json")).IsSuccessStatusCode.Should().BeTrue();
        (await Ctx.Client.ExportAsync(submit.Batch_Id, "csv")).IsSuccessStatusCode.Should().BeTrue();
        (await Ctx.Client.ExportAsync(submit.Batch_Id, "txt")).IsSuccessStatusCode.Should().BeTrue();

        // optional: balance after (assert non-increase until you assert exact delta)
        var balAfter = await Ctx.Client.GetBalanceAsync();
        balAfter.IsSuccessStatusCode.Should().BeTrue();
    }
}
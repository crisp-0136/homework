using FluentAssertions;
using NUnit.Framework;
using Scriptube.API.Tests.Framework.Http;

namespace Scriptube.API.Tests.Tests.Regression;

[TestFixture]
[Category("API")]
[Category("Regression")]
[NonParallelizable]
public sealed class Regression_Credits_Deduct_For_ManualCaptions : BaseApiTest
{
    [Test]
    public async Task Manual_caption_video_should_cost_4_credits()
    {
        var video = "https://www.youtube.com/watch?v=tstENMAN001";

        var beforeRes = await Ctx.Client.GetBalanceAsync();
        beforeRes.IsSuccessStatusCode.Should().BeTrue();
        var before = BalanceExtractor.FromJson(await beforeRes.Content.ReadAsStringAsync());

        var submitRes = await Ctx.Client.SubmitTranscriptsAsync(new
        {
            urls = new[] { video },
            translate_to_english = false,
            use_byok = false
        });

        ((int)submitRes.StatusCode).Should().Be(202);
        var batchId = BatchIdExtractor.FromJson(await submitRes.Content.ReadAsStringAsync());

        var terminal = await Polling.WaitForBatchTerminalAsync(
            () => Ctx.Client.GetBatchAsync(batchId),
            timeout: TimeSpan.FromMinutes(3),
            interval: TimeSpan.FromSeconds(2));

        terminal.GetProperty("status").GetString()!.Should().Be("completed");

        var afterRes = await Ctx.Client.GetBalanceAsync();
        afterRes.IsSuccessStatusCode.Should().BeTrue();
        var after = BalanceExtractor.FromJson(await afterRes.Content.ReadAsStringAsync());

        after.Should().Be(before - Ctx.Settings.MinVideoCostCredits);
    }
}
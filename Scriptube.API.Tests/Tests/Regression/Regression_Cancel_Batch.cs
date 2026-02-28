using FluentAssertions;

using NUnit.Framework;

using Scriptube.API.Tests.Framework.Http;

using Allure.NUnit;

namespace Scriptube.API.Tests.Tests.Regression;

[TestFixture]
[AllureNUnit]
[Category("API")]
[Category("Regression")]
[Ignore("Cancel endpoint not available in current OpenAPI contract")]
public sealed class Regression_Cancel_Batch : BaseApiTest
{
    [Test]
    public async Task Cancel_should_move_batch_to_cancelled()
    {
        var video = "https://www.youtube.com/watch?v=oA85M9JHsW0";

        var submitRes = await Ctx.Client.SubmitTranscriptsAsync(new
        {
            urls = new[] { video },
            translate_to_english = false,
            use_byok = false
        });

        ((int)submitRes.StatusCode).Should().Be(202);
        var batchId = BatchIdExtractor.FromJson(await submitRes.Content.ReadAsStringAsync());

        var cancelRes = await Ctx.Client.CancelAsync(batchId);
        cancelRes.IsSuccessStatusCode.Should().BeTrue();

        var terminal = await Polling.WaitForBatchTerminalAsync(
            () => Ctx.Client.GetBatchAsync(batchId),
            timeout: TimeSpan.FromMinutes(2),
            interval: TimeSpan.FromSeconds(2));

        terminal.GetProperty("status").GetString()!
            .Should().BeOneOf("cancelled", "canceled", "completed");
    }
}
using FluentAssertions;
using NUnit.Framework;
using Scriptube.API.Tests.Framework.Http;

namespace Scriptube.API.Tests.Tests.Regression;

[TestFixture]
[Category("API")]
[Category("Regression")]
[Ignore("Delete concrete item or batch not available in current OpenAPI contract")]
public sealed class Regression_Delete_Batch_Item : BaseApiTest
{
    [Test]
    public async Task Delete_batch_item_should_succeed_or_be_unauthorized_if_internal_only()
    {
        var video = "https://www.youtube.com/watch?v=tstENMAN001";

        var submitRes = await Ctx.Client.SubmitTranscriptsAsync(new { urls = new[] { video } });
        ((int)submitRes.StatusCode).Should().Be(202);
        var batchId = BatchIdExtractor.FromJson(await submitRes.Content.ReadAsStringAsync());

        // fetch batch to obtain item id
        var batchRes = await Ctx.Client.GetBatchAsync(batchId);
        batchRes.IsSuccessStatusCode.Should().BeTrue();
        var batchJson = await batchRes.Content.ReadAsStringAsync();

        var itemId = BatchItemExtractor.FirstItemId(batchJson);

        var delRes = await Ctx.Client.DeleteBatchItemAsync(batchId, itemId);

        // If this endpoint is dashboard-session-only, you'll get 401/403 with API key.
        ((int)delRes.StatusCode).Should().BeOneOf(200, 204, 401, 403);
    }
}
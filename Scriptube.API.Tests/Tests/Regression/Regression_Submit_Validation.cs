using FluentAssertions;
using NUnit.Framework;
using Scriptube.API.Tests.Framework.Http;
using Allure.NUnit;
namespace Scriptube.API.Tests.Tests.Regression;

[TestFixture]
[AllureNUnit]
[Category("API")]
[Category("Regression")]
[NonParallelizable]
public sealed class Regression_Submit_Validation : BaseApiTest
{
    [Test]
    public async Task Empty_urls_should_return_400()
    {
        var res = await Ctx.Client.SubmitTranscriptsAsync(new { urls = Array.Empty<string>() });
        ((int)res.StatusCode).Should().BeOneOf(400, 422);
    }

    [Test]
    public async Task Non_youtube_url_should_return_400()
    {
        var res = await Ctx.Client.SubmitTranscriptsAsync(new { urls = new[] { "https://invalid.invalid" } });
        ((int)res.StatusCode).Should().BeOneOf(400, 422);
    }

    [Test]
    public async Task Nonexistent_batch_should_return_422()
    {
        var res = await Ctx.Client.GetBatchAsync("does-not-exist");
        ((int)res.StatusCode).Should().Be(422);
    }
}
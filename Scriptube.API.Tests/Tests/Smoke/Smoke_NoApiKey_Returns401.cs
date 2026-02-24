using FluentAssertions;
using NUnit.Framework;
using Scriptube.API.Tests.Framework.Clients;
using Scriptube.API.Tests.Framework.Config;
using Scriptube.API.Tests.Framework.Http;

namespace Scriptube.API.Tests.Tests.Smoke;

[TestFixture]
[Category("API")]
[Category("Smoke")]
public sealed class Smoke_NoApiKey_Returns401
{
    [Test]
    public async Task Credits_balance_without_key_should_return_401()
    {
        var settings = TestConfig.Load() with { }; // load base URL/timeouts
        var handler = new HttpLoggingHandler(_ => { }) { InnerHandler = new HttpClientHandler() };
        using var http = new HttpClient(handler) { BaseAddress = new Uri(settings.BaseUrl) };
        var client = new ScriptubeApiClient(http, settings with { ApiKey = "" });

        var res = await client.GetBalanceAsync();
        ((int)res.StatusCode).Should().Be(401);
    }
}
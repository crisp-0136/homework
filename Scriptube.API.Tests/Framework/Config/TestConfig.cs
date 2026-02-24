using Microsoft.Extensions.Configuration;

namespace Scriptube.API.Tests.Framework.Config;

public static class TestConfig
{
    public static ApiSettings Load()
    {
        var cfg = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables(prefix: "SCRIPTUBE_")
            .Build();

        var api = cfg.GetRequiredSection("Api");
        var secrets = cfg.GetRequiredSection("Secrets");
        var assertions = cfg.GetSection("Assertions");

        return new ApiSettings
        {
            BaseUrl = api.GetValue<string>("BaseUrl") ?? throw new InvalidOperationException("Api:BaseUrl missing"),
            TimeoutMs = api.GetValue("TimeoutMs", 30_000),
            RetryCount = api.GetValue("RetryCount", 2),
            RetryDelayMs = api.GetValue("RetryDelayMs", 500),
            ApiKey = secrets.GetValue<string>("ApiKey") ?? "",
            MinVideoCostCredits = assertions.GetValue("MinVideoCostCredits", 4)
        };
    }
}
using Microsoft.Extensions.Configuration;

namespace Scriptube.UI.Tests.Framework.Config;

public static class TestConfig
{
    public static UiSettings LoadUiSettings()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables(prefix: "SCRIPTUBE_")
            .Build();

        var env = config.GetRequiredSection("Environment");
        var auth = config.GetRequiredSection("Auth");

        return new UiSettings
        {
            BaseUrl = env["BaseUrl"] ?? throw new InvalidOperationException("Environment:BaseUrl missing"),
            Headless = bool.TryParse(env["Headless"], out var h) ? h : true,
            TimeoutMs = int.TryParse(env["TimeoutMs"], out var t) ? t : 30_000,
            SlowMoMs = int.TryParse(env["SlowMoMs"], out var s) ? s : 0,
            Auth = new AuthSettings
            {
                Email = auth["Email"] ?? "",
                Password = auth["Password"] ?? ""
            }
        };
    }
}
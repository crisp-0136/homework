using Microsoft.Extensions.Configuration;
using Scriptube.Webhook.Tests.Config;

namespace Scriptube.Webhook.Tests.TestBase;

public static class TestConfiguration
{
    private static readonly Lazy<IConfiguration> _config = new(() =>
    new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
        .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false)
        .AddEnvironmentVariables()
        .Build());

    public static ScriptubeSettings Scriptube => Bind<ScriptubeSettings>("Api", "Secrets");
    public static WebhookTestSettings Webhook => Bind<WebhookTestSettings>("Receiver");

    private static T Bind<T>(params string[] sections) where T : new()
    {
        var obj = new T();
        var cfg = _config.Value;

        if (sections.Length == 1)
        {
            cfg.GetSection(sections[0]).Bind(obj);
            return obj;
        }

        // Merge multiple sections into one object (Api + Secrets)
        foreach (var s in sections) cfg.GetSection(s).Bind(obj);
        return obj;
    }
}
using Microsoft.Playwright;

using Scriptube.UI.Tests.Framework.Config;

namespace Scriptube.UI.Tests.Framework.Browser;

public static class PlaywrightFactory
{
    public static async Task<IBrowser> LaunchChromiumAsync(IPlaywright pw, UiSettings settings)
    {
        return await pw.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = settings.Headless,
            SlowMo = settings.SlowMoMs > 0 ? settings.SlowMoMs : null
        });
    }

    public static async Task<IBrowserContext> NewContextAsync(IBrowser browser, UiSettings settings)
    {
        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1366, Height = 768 }
        });

        context.SetDefaultTimeout(settings.TimeoutMs);
        context.SetDefaultNavigationTimeout(settings.TimeoutMs);

        return context;
    }

    public static async Task<IPage> NewPageAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        return page;
    }
}
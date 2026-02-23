using Microsoft.Playwright;
using NUnit.Framework;
using Scriptube.UI.Tests.Framework.Config;

namespace Scriptube.UI.Tests.Framework.Browser;

[Parallelizable(ParallelScope.All)]
public abstract class BaseUiTest
{
    protected UiSettings Settings = null!;
    protected IPlaywright Playwright = null!;
    protected IBrowser Browser = null!;
    protected IBrowserContext Context = null!;
    protected IPage Page = null!;

    [SetUp]
    public async Task SetUp()
    {
        Settings = TestConfig.LoadUiSettings();

        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await PlaywrightFactory.LaunchChromiumAsync(Playwright, Settings);
        Context = await PlaywrightFactory.NewContextAsync(Browser, Settings);
        Page = await PlaywrightFactory.NewPageAsync(Context);
    }

    [TearDown]
    public async Task TearDown()
    {
        try
        {
            var failed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed;
            if (failed && Page is not null)
            {
                var dir = Path.Combine(TestContext.CurrentContext.WorkDirectory, "artifacts", "screenshots");
                Directory.CreateDirectory(dir);

                var file = $"{Sanitize(TestContext.CurrentContext.Test.FullName)}.png";
                var path = Path.Combine(dir, file);

                await Page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = path,
                    FullPage = true
                });

                TestContext.AddTestAttachment(path, "UI failure screenshot");
            }
        }
        catch
        {
            // never fail teardown
        }
        finally
        {
            if (Context is not null) await Context.DisposeAsync();
            if (Browser is not null) await Browser.DisposeAsync();
            Playwright?.Dispose();
        }
    }

    protected async Task GotoBaseAsync(string relative = "/")
    {
        var url = new Uri(new Uri(Settings.BaseUrl), relative).ToString();
        await Page.GotoAsync(url);
    }

    private static string Sanitize(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name;
    }
}
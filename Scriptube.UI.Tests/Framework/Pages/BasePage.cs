using Microsoft.Playwright;

using Scriptube.UI.Tests.Framework.Config;

namespace Scriptube.UI.Tests.Framework.Pages;

public abstract class BasePage
{
    protected BasePage(IPage page, UiSettings settings)
    {
        Page = page;
        Settings = settings;
    }

    protected IPage Page { get; }
    protected UiSettings Settings { get; }

    protected string Url(string relative) => new Uri(new Uri(Settings.BaseUrl), relative).ToString();

    public async Task GotoAsync(string relative)
        => await Page.GotoAsync(Url(relative));
}
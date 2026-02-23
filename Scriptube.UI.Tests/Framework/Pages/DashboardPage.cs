using Microsoft.Playwright;
using Scriptube.UI.Tests.Framework.Config;

namespace Scriptube.UI.Tests.Framework.Pages;

public sealed class DashboardPage : BasePage
{
    public DashboardPage(IPage page, UiSettings settings) : base(page, settings) { }

    private ILocator Anchor => Page.Locator("a[data-event-id='dashboard']");

    public async Task<bool> IsLoadedAsync()
        => await Anchor.IsVisibleAsync();

    private ILocator Logout => Page.Locator("a[href='/ui/logout']");
    public async Task LogoutAsync()
    {
        await Logout.ClickAsync();
    }
}
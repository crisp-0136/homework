using Microsoft.Playwright;
using Scriptube.UI.Tests.Framework.Config;

namespace Scriptube.UI.Tests.Framework.Pages;

public sealed class LoginPage : BasePage
{
    public LoginPage(IPage page, UiSettings settings) : base(page, settings) { }

    private ILocator Email => Page.Locator("#email");
    private ILocator Password => Page.Locator("#password");
    private ILocator Submit => Page.Locator("#submit-btn");
    private ILocator EmailError => Page.Locator("#email-error");
    private ILocator PasswordError => Page.Locator("#password-error");
    private ILocator FormError => Page.Locator("div.alert.alert-error:has-text('Invalid credentials')");

    public async Task OpenAsync() => await GotoAsync("/ui/login");

    public async Task LoginAsync(string email, string password)
    {
        await Email.FillAsync(email);
        await Password.FillAsync(password);

        await Task.WhenAll(
        Page.WaitForLoadStateAsync(LoadState.NetworkIdle),
        Submit.ClickAsync()
        );
    }

    public async Task<bool> HasInvalidCredentialsErrorAsync()
        => await FormError.IsVisibleAsync();
    public async Task<bool> HasInlineErrorAsync()
        => (await EmailError.IsVisibleAsync()) || (await PasswordError.IsVisibleAsync());
}

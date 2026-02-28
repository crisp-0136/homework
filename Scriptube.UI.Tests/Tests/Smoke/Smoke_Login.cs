using FluentAssertions;

using NUnit.Framework;

using Scriptube.UI.Tests.Framework.Browser;
using Scriptube.UI.Tests.Framework.Pages;

using Allure.NUnit;

namespace Scriptube.UI.Tests.Tests.Smoke;

[TestFixture]
[AllureNUnit]
[Category("UI")]
[Category("Smoke")]
public sealed class Smoke_Login : BaseUiTest
{
    [Test]
    public async Task Login_should_load_dashboard()
    {
        Settings.Auth.Email.Should().NotBeNullOrWhiteSpace();
        Settings.Auth.Password.Should().NotBeNullOrWhiteSpace();

        var login = new LoginPage(Page, Settings);
        await login.OpenAsync();
        await login.LoginAsync(Settings.Auth.Email, Settings.Auth.Password);

        var dashboard = new DashboardPage(Page, Settings);
        (await dashboard.IsLoadedAsync()).Should().BeTrue("dashboard anchor must be visible after login");
        (await login.HasInlineErrorAsync()).Should().BeFalse();
    }
}
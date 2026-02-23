using FluentAssertions;
using NUnit.Framework;
using Scriptube.UI.Tests.Framework.Browser;
using Scriptube.UI.Tests.Framework.Pages;

namespace Scriptube.UI.Tests.Tests.Smoke;

[TestFixture]
[Category("UI")]
[Category("Smoke")]
public sealed class Smoke_Login_Invalid : BaseUiTest
{
    [Test]
    public async Task Invalid_login_should_show_error()
    {
        var login = new LoginPage(Page, Settings);
        await login.OpenAsync();
        await login.LoginAsync("wrong@example.com", "wrong-password");

        (await login.HasInvalidCredentialsErrorAsync()).Should().BeTrue();
    }
}
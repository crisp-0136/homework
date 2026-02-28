using FluentAssertions;

using NUnit.Framework;

using Scriptube.UI.Tests.Framework.Browser;

using Allure.NUnit;
namespace Scriptube.UI.Tests.Tests.Smoke;

[TestFixture]
[AllureNUnit]
[Category("UI")]
[Category("Smoke")]
public sealed class Smoke_Homepage_Loads : BaseUiTest
{
    [Test]
    public async Task Homepage_should_load()
    {
        await GotoBaseAsync("/");
        var title = await Page.TitleAsync();
        title.Should().NotBeNullOrWhiteSpace();
    }
}
using Allure.NUnit;

using NUnit.Framework;

namespace Scriptube.UI.Tests;

[SetUpFixture]
public sealed class AllureGlobalSetup
{
    [OneTimeSetUp]
    public void Init() { }
}
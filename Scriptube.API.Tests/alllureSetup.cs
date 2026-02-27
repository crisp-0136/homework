using Allure.NUnit;
using NUnit.Framework;

namespace Scriptube.API.Tests;

[SetUpFixture]
public sealed class AllureGlobalSetup
{
    [OneTimeSetUp]
    public void Init() { }
}
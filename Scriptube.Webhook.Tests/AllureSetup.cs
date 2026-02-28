using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Scriptube.Webhook.Tests;

[SetUpFixture]
public sealed class AllureGlobalSetup
{
    [OneTimeSetUp]
    public void Init() { }
}
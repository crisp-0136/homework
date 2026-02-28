using NUnit.Framework;

using Scriptube.API.Tests.Framework.Config;

namespace Scriptube.API.Tests.Framework.Http;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public abstract class BaseApiTest
{
    protected ApiTestContext Ctx = null!;

    [SetUp]
    public void SetUp()
    {
        var settings = TestConfig.Load();
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
            Assert.Inconclusive("API key missing. Provide SCRIPTUBE_Secrets__ApiKey or appsettings.local.json.");

        Ctx = new ApiTestContext(settings);
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            var dir = Path.Combine(TestContext.CurrentContext.WorkDirectory, "artifacts", "api-logs");
            Directory.CreateDirectory(dir);

            var file = $"{Sanitize(TestContext.CurrentContext.Test.FullName)}.log";
            var path = Path.Combine(dir, file);

            File.WriteAllText(path, string.Join(Environment.NewLine + Environment.NewLine, Ctx.Logs));
            TestContext.AddTestAttachment(path, "API request/response log");
        }
        finally
        {
            Ctx.Dispose();
        }
    }

    private static string Sanitize(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name;
    }
}
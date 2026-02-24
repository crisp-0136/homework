using Scriptube.API.Tests.Framework.Clients;
using Scriptube.API.Tests.Framework.Config;

namespace Scriptube.API.Tests.Framework.Http;

public sealed class ApiTestContext : IDisposable
{
    public ApiSettings Settings { get; }
    public List<string> Logs { get; } = new();

    public ScriptubeApiClient Client { get; }

    private readonly HttpClient _http;

    public ApiTestContext(ApiSettings settings)
    {
        Settings = settings;

        var handler = new HttpLoggingHandler(s => Logs.Add(s))
        {
            InnerHandler = new HttpClientHandler()
        };

        _http = new HttpClient(handler);
        Client = new ScriptubeApiClient(_http, settings);
    }

    public void Dispose() => _http.Dispose();
}
using System.Net;

namespace Scriptube.Webhook.Tests.Clients;

public sealed class ScriptubeAuthClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public ScriptubeAuthClient(string baseUrl, string apiKey)
    {
        _http = HttpClientFactory.Create(baseUrl);
        _apiKey = apiKey;
    }

    public async Task EnsureApiKeyValidAsync(CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "api/v1/user");
        req.Headers.TryAddWithoutValidation("X-API-Key", _apiKey);

        using var resp = await _http.SendAsync(req, ct);

        if (resp.StatusCode == HttpStatusCode.Unauthorized)
        {
            var body = await resp.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException(
                $"Scriptube API key rejected (401). Response: {body}");
        }

        resp.EnsureSuccessStatusCode();
    }
}
using System.Net.Http.Json;
using Scriptube.Webhook.Tests.Models;

namespace Scriptube.Webhook.Tests.Clients;

public sealed class ReceiverAdminClient
{
    private readonly HttpClient _http;
    private readonly string _adminToken;

    public ReceiverAdminClient(string receiverBaseUrl, string adminToken)
    {
        _http = HttpClientFactory.Create(receiverBaseUrl);
        _adminToken = adminToken;
    }

    public async Task ClearAsync(CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "clear");
        req.Headers.TryAddWithoutValidation("X-Admin-Token", _adminToken);
        using var resp = await _http.SendAsync(req, ct);
        resp.EnsureSuccessStatusCode();
    }

    public async Task<HookEvent?> GetLastAsync(CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "last");
        req.Headers.TryAddWithoutValidation("X-Admin-Token", _adminToken);
        using var resp = await _http.SendAsync(req, ct);
        if (!resp.IsSuccessStatusCode) return null;

        return await resp.Content.ReadFromJsonAsync<HookEvent>(cancellationToken: ct);
    }

    public async Task<IReadOnlyList<HookEvent>> GetAllAsync(CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "all");
        req.Headers.TryAddWithoutValidation("X-Admin-Token", _adminToken);
        using var resp = await _http.SendAsync(req, ct);
        if (!resp.IsSuccessStatusCode) return Array.Empty<HookEvent>();

        var items = await resp.Content.ReadFromJsonAsync<List<HookEvent>>(cancellationToken: ct);
        return items ?? [];
    }
}
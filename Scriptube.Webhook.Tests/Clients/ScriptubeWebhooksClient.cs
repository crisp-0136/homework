using System.Net;
using System.Text.Json;
using System.Net.Http.Json;
using Scriptube.Webhook.Tests.Models;

namespace Scriptube.Webhook.Tests.Clients;

public sealed class ScriptubeWebhooksClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public ScriptubeWebhooksClient(string scriptubeBaseUrl, string apiKey)
    {
        _http = HttpClientFactory.Create(scriptubeBaseUrl);
        _apiKey = apiKey;
    }

    private HttpRequestMessage NewReq(HttpMethod method, string path)
    {
        var req = new HttpRequestMessage(method, path);
        req.Headers.TryAddWithoutValidation("X-API-Key", _apiKey);
        return req;
    }

    public async Task<RegisterWebhookResponse> RegisterAsync(RegisterWebhookRequest request, CancellationToken ct = default)
    {
    using var req = NewReq(HttpMethod.Post, "api/webhooks/register");
    req.Content = JsonContent.Create(request);

    using var resp = await _http.SendAsync(req, ct);
    var body = await resp.Content.ReadAsStringAsync(ct);

    if (!resp.IsSuccessStatusCode)
        throw new HttpRequestException($"Register webhook failed: {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");

    // tolerant extraction
    using var doc = JsonDocument.Parse(body);
    var root = doc.RootElement;

    // unwrap common wrappers
    if (root.ValueKind == JsonValueKind.Object)
    {
        if (root.TryGetProperty("data", out var d) && d.ValueKind == JsonValueKind.Object)
            root = d;
        else if (root.TryGetProperty("webhook", out var w) && w.ValueKind == JsonValueKind.Object)
            root = w;
    }

    string? id = null;

    if (root.ValueKind == JsonValueKind.Object)
    {
        if (root.TryGetProperty("id", out var a) && a.ValueKind == JsonValueKind.String) id = a.GetString();
        else if (root.TryGetProperty("webhook_id", out var b) && b.ValueKind == JsonValueKind.String) id = b.GetString();
    }

    if (string.IsNullOrWhiteSpace(id))
        throw new InvalidOperationException($"Register succeeded but no webhook id in response. Body: {body}");

    return new RegisterWebhookResponse { Id = id! };
    }

    public async Task TestAsync(string webhookId, CancellationToken ct = default)
    {
        using var req = NewReq(HttpMethod.Post, $"api/webhooks/{Uri.EscapeDataString(webhookId)}/test");
        using var resp = await _http.SendAsync(req, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
            throw new HttpRequestException($"Webhook test failed: {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");
    }

    public async Task<IReadOnlyList<AvailableWebhookEvent>> GetAvailableEventsAsync(CancellationToken ct = default)
    {
    using var req = NewReq(HttpMethod.Get, "api/webhooks/events/available");
    using var resp = await _http.SendAsync(req, ct);
    var body = await resp.Content.ReadAsStringAsync(ct);

    if (!resp.IsSuccessStatusCode)
        throw new HttpRequestException($"Get available events failed: {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");

    using var doc = JsonDocument.Parse(body);
    var root = doc.RootElement;

    // unwrap common wrappers
    if (root.ValueKind == JsonValueKind.Object)
    {
        if (root.TryGetProperty("events", out var e)) root = e;
        else if (root.TryGetProperty("data", out var d)) root = d;
    }

    if (root.ValueKind != JsonValueKind.Array)
        return new List<AvailableWebhookEvent>();

    var result = new List<AvailableWebhookEvent>();

    foreach (var el in root.EnumerateArray())
    {
        if (el.ValueKind == JsonValueKind.String)
        {
            var s = el.GetString();
            if (!string.IsNullOrWhiteSpace(s))
                result.Add(new AvailableWebhookEvent { Name = s });
            continue;
        }

        if (el.ValueKind == JsonValueKind.Object)
        {
            // try common keys
            string? name = null;

            if (el.TryGetProperty("name", out var n) && n.ValueKind == JsonValueKind.String) name = n.GetString();
            else if (el.TryGetProperty("event", out var ev) && ev.ValueKind == JsonValueKind.String) name = ev.GetString();
            else if (el.TryGetProperty("type", out var t) && t.ValueKind == JsonValueKind.String) name = t.GetString();

            string? desc = null;
            if (el.TryGetProperty("description", out var ds) && ds.ValueKind == JsonValueKind.String) desc = ds.GetString();

            if (!string.IsNullOrWhiteSpace(name))
                result.Add(new AvailableWebhookEvent { Name = name!, Description = desc });

            continue;
        }
    }

    return result;
    }

    public async Task<WebhookDeliveryLogsResponse> GetLogsAsync(string webhookId, CancellationToken ct = default)
    {
    using var req = NewReq(HttpMethod.Get, $"api/webhooks/{Uri.EscapeDataString(webhookId)}/logs");
    using var resp = await _http.SendAsync(req, ct);
    var body = await resp.Content.ReadAsStringAsync(ct);

    if (!resp.IsSuccessStatusCode)
        throw new HttpRequestException($"Get logs failed: {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");

    return JsonSerializer.Deserialize<WebhookDeliveryLogsResponse>(body, JsonOpts)
           ?? new WebhookDeliveryLogsResponse();
    }

    public async Task DeleteAsync(string webhookId, CancellationToken ct = default)
    {
        using var req = NewReq(HttpMethod.Delete, $"api/webhooks/{Uri.EscapeDataString(webhookId)}");
        using var resp = await _http.SendAsync(req, ct);

        if (resp.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.MethodNotAllowed)
            return;

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"Delete webhook failed: {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");
        }
    }
    public async Task<IReadOnlyList<WebhookInfo>> ListAsync(CancellationToken ct = default)
    {
    using var req = NewReq(HttpMethod.Get, "api/webhooks");
    using var resp = await _http.SendAsync(req, ct);
    var body = await resp.Content.ReadAsStringAsync(ct);

    if (!resp.IsSuccessStatusCode)
        throw new HttpRequestException($"List webhooks failed: {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");

    using var doc = JsonDocument.Parse(body);
    var root = doc.RootElement;

    if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty("webhooks", out var arr) || arr.ValueKind != JsonValueKind.Array)
        return new List<WebhookInfo>();

    var list = JsonSerializer.Deserialize<List<WebhookInfo>>(arr.GetRawText(), JsonOpts);
    return list ?? new List<WebhookInfo>();
    }
}
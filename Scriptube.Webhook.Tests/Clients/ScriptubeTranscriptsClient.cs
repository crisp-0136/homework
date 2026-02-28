using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Scriptube.Webhook.Tests.Clients;

public sealed class ScriptubeTranscriptsClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public ScriptubeTranscriptsClient(string scriptubeBaseUrl, string apiKey)
    {
        _http = HttpClientFactory.Create(scriptubeBaseUrl, timeout: TimeSpan.FromSeconds(120));
        _apiKey = apiKey;
    }

    public sealed record SubmitTranscriptsRequest(
        List<string> urls,
        bool use_byok = false
    );

    private HttpRequestMessage NewReq(HttpMethod method, string path)
    {
        var req = new HttpRequestMessage(method, path);
        req.Headers.TryAddWithoutValidation("X-API-Key", _apiKey);
        return req;
    }

    public async Task<string> SubmitAsync(List<string> urls, bool useByok, CancellationToken ct = default)
    {
        using var req = NewReq(HttpMethod.Post, "api/v1/transcripts");
        req.Content = JsonContent.Create(new SubmitTranscriptsRequest(urls, useByok));

        using var resp = await _http.SendAsync(req, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);

        if (resp.StatusCode == HttpStatusCode.Unauthorized)
            throw new HttpRequestException($"Submit transcripts unauthorized (401). Body: {body}");

        resp.EnsureSuccessStatusCode();

        // Tolerant parse: accept common batch id fields.
        using var doc = JsonDocument.Parse(body);
        if (doc.RootElement.TryGetProperty("batch_id", out var a)) return a.GetString() ?? "";
        if (doc.RootElement.TryGetProperty("batchId", out var b)) return b.GetString() ?? "";
        if (doc.RootElement.TryGetProperty("id", out var c)) return c.GetString() ?? "";
        throw new InvalidOperationException("Submit response missing batch id");
    }

    public async Task<JsonDocument> GetBatchAsync(string batchId, CancellationToken ct = default)
    {
        using var req = NewReq(HttpMethod.Get, $"api/v1/transcripts/{Uri.EscapeDataString(batchId)}");
        using var resp = await _http.SendAsync(req, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);

        if (resp.StatusCode == HttpStatusCode.Unauthorized)
            throw new HttpRequestException($"Get batch unauthorized (401). Body: {body}");

        resp.EnsureSuccessStatusCode();
        return JsonDocument.Parse(body);
    }

    public async Task WaitUntilCompletedAsync(
        string batchId,
        TimeSpan timeout,
        TimeSpan pollInterval,
        CancellationToken ct = default)
    {
        var deadline = DateTimeOffset.UtcNow + timeout;

        while (DateTimeOffset.UtcNow < deadline)
        {
            using var doc = await GetBatchAsync(batchId, ct);

            if (TryGetStatus(doc.RootElement, out var status))
            {
                if (status is "completed" or "complete" or "done") return;
                if (status is "failed" or "error")
                    throw new InvalidOperationException($"Batch failed: {batchId}");
            }

            await Task.Delay(pollInterval, ct);
        }

        throw new TimeoutException($"Batch did not complete within {timeout.TotalSeconds:0}s: {batchId}");
    }

    private static bool TryGetStatus(JsonElement root, out string status)
    {
        status = "";

        if (root.TryGetProperty("status", out var s) && s.ValueKind == JsonValueKind.String)
        {
            status = (s.GetString() ?? "").Trim().ToLowerInvariant();
            return true;
        }

        // Some APIs wrap payloads; keep tolerant
        if (root.TryGetProperty("data", out var data) &&
            data.ValueKind == JsonValueKind.Object &&
            data.TryGetProperty("status", out var ds) &&
            ds.ValueKind == JsonValueKind.String)
        {
            status = (ds.GetString() ?? "").Trim().ToLowerInvariant();
            return true;
        }

        return false;
    }
}
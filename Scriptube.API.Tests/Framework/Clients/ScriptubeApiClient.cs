using System.Net.Http.Json;
using Scriptube.API.Tests.Framework.Config;

namespace Scriptube.API.Tests.Framework.Clients;

public sealed class ScriptubeApiClient
{
    private readonly HttpClient _http;

    public ScriptubeApiClient(HttpClient http, ApiSettings settings)
    {
        _http = http;
        _http.BaseAddress = new Uri(settings.BaseUrl);
        _http.Timeout = TimeSpan.FromMilliseconds(settings.TimeoutMs);

        if (!string.IsNullOrWhiteSpace(settings.ApiKey))
            _http.DefaultRequestHeaders.Add("X-API-Key", settings.ApiKey);
    }

    public async Task<HttpResponseMessage> GetBalanceAsync(CancellationToken ct = default)
        => await _http.GetAsync("/api/v1/credits/balance", ct);

    public async Task<HttpResponseMessage> PrecheckAsync(object payload, CancellationToken ct = default)
        => await _http.PostAsJsonAsync("/api/v1/credits/precheck", payload, ct);

    public async Task<HttpResponseMessage> SubmitTranscriptsAsync(object payload, CancellationToken ct = default)
        => await _http.PostAsJsonAsync("/api/v1/transcripts", payload, ct);

    public async Task<HttpResponseMessage> GetBatchAsync(string batchId, CancellationToken ct = default)
        => await _http.GetAsync($"/api/v1/transcripts/{batchId}", ct);

    public async Task<HttpResponseMessage> ExportAsync(string batchId, string format, CancellationToken ct = default)
        => await _http.GetAsync($"/api/v1/transcripts/{batchId}/export?format={Uri.EscapeDataString(format)}", ct);
    public Task<HttpResponseMessage> CancelAsync(string batchId, CancellationToken ct = default)
    => _http.PostAsync($"/api/v1/transcripts/{batchId}/cancel", content: null, ct);

    public Task<HttpResponseMessage> RetryFailedAsync(string batchId, CancellationToken ct = default)
        => _http.PostAsync($"/api/v1/transcripts/{batchId}/retry-failed", content: null, ct);

    public Task<HttpResponseMessage> RerunAsync(string batchId, CancellationToken ct = default)
        => _http.PostAsync($"/api/v1/transcripts/{batchId}/rerun", content: null, ct);

    public Task<HttpResponseMessage> DeleteBatchItemAsync(string batchId, string itemId, CancellationToken ct = default)
    => _http.DeleteAsync($"/api/batches/{batchId}/items/{itemId}", ct);

}
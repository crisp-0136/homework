using System.Net.Http.Headers;
using System.Text;

namespace Scriptube.API.Tests.Framework.Http;

public sealed class HttpLoggingHandler : DelegatingHandler
{
    private readonly Action<string> _log;
    private readonly string _apiKeyHeaderName;

    public HttpLoggingHandler(Action<string> log, string apiKeyHeaderName = "X-API-Key")
    {
        _log = log;
        _apiKeyHeaderName = apiKeyHeaderName;
    }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            _log(await FormatRequestSafe(request));

            var response = await base.SendAsync(request, ct);

            _log(await FormatResponseSafe(response));
            _log($"<-- END ({(int)response.StatusCode}) in {sw.ElapsedMilliseconds}ms");

            return response;
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            _log($"<-- TIMEOUT {request.Method} {request.RequestUri} after {sw.ElapsedMilliseconds}ms");
            throw;
        }
        catch (Exception ex)
        {
            _log($"<-- ERROR {request.Method} {request.RequestUri} after {sw.ElapsedMilliseconds}ms: {ex.GetType().Name}: {ex.Message}");
            throw;
        }
    }

    private static async Task<string> FormatRequestSafe(HttpRequestMessage request)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($">> {request.Method} {request.RequestUri}");

        foreach (var h in request.Headers)
            sb.AppendLine($"{h.Key}: {string.Join(",", h.Value)}");

        if (request.Content is not null)
        {
            foreach (var h in request.Content.Headers)
                sb.AppendLine($"{h.Key}: {string.Join(",", h.Value)}");

            // Read without permanently consuming: buffer then restore
            var bytes = await request.Content.ReadAsByteArrayAsync();
            var body = System.Text.Encoding.UTF8.GetString(bytes);

            sb.AppendLine();
            sb.AppendLine(body);

            var newContent = new ByteArrayContent(bytes);
            foreach (var h in request.Content.Headers)
                newContent.Headers.TryAddWithoutValidation(h.Key, h.Value);

            request.Content = newContent;
        }

        sb.AppendLine(">> END");
        return sb.ToString();
    }

    private static async Task<string> FormatResponseSafe(HttpResponseMessage response)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"<-- {(int)response.StatusCode} {response.ReasonPhrase}");

        foreach (var h in response.Headers)
            sb.AppendLine($"{h.Key}: {string.Join(",", h.Value)}");

        if (response.Content is not null)
        {
            foreach (var h in response.Content.Headers)
                sb.AppendLine($"{h.Key}: {string.Join(",", h.Value)}");

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var body = System.Text.Encoding.UTF8.GetString(bytes);

            sb.AppendLine();
            sb.AppendLine(body);

            var newContent = new ByteArrayContent(bytes);
            foreach (var h in response.Content.Headers)
                newContent.Headers.TryAddWithoutValidation(h.Key, h.Value);

            response.Content = newContent;
        }

        return sb.ToString();
    }

    private string MaskHeader(string key, string value)
        => key.Equals(_apiKeyHeaderName, StringComparison.OrdinalIgnoreCase) ? "****" : value;
}
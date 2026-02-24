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
        _log(await FormatRequest(request));

        var response = await base.SendAsync(request, ct);

        _log(await FormatResponse(response));
        return response;
    }

    private async Task<string> FormatRequest(HttpRequestMessage req)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"--> {req.Method} {req.RequestUri}");

        foreach (var h in req.Headers)
            sb.AppendLine($"{h.Key}: {MaskHeader(h.Key, string.Join(", ", h.Value))}");

        if (req.Content is not null)
        {
            foreach (var h in req.Content.Headers)
                sb.AppendLine($"{h.Key}: {string.Join(", ", h.Value)}");

            var body = await req.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(body))
                sb.AppendLine(body);
        }

        sb.AppendLine("--> END");
        return sb.ToString();
    }

    private async Task<string> FormatResponse(HttpResponseMessage res)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"<-- {(int)res.StatusCode} {res.ReasonPhrase}");

        foreach (var h in res.Headers)
            sb.AppendLine($"{h.Key}: {string.Join(", ", h.Value)}");

        if (res.Content is not null)
        {
            foreach (var h in res.Content.Headers)
                sb.AppendLine($"{h.Key}: {string.Join(", ", h.Value)}");

            var body = await res.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(body))
                sb.AppendLine(body);
        }

        sb.AppendLine("<-- END");
        return sb.ToString();
    }

    private string MaskHeader(string key, string value)
        => key.Equals(_apiKeyHeaderName, StringComparison.OrdinalIgnoreCase) ? "****" : value;
}
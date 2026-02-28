using System.Net.Http.Headers;

namespace Scriptube.Webhook.Tests.Clients;

public static class HttpClientFactory
{
    public static HttpClient Create(string baseUrl, TimeSpan? timeout = null)
    {
        var http = new HttpClient
        {
            BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/"),
            Timeout = timeout ?? TimeSpan.FromSeconds(60),
        };

        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return http;
    }
}
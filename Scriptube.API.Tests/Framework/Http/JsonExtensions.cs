using System.Text.Json;

namespace Scriptube.API.Tests.Framework.Http;

public static class JsonExtensions
{
    private static readonly JsonSerializerOptions Opt = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<T> ReadAsAsync<T>(this HttpContent content)
    {
        var s = await content.ReadAsStringAsync();
        var obj = JsonSerializer.Deserialize<T>(s, Opt);
        return obj ?? throw new InvalidOperationException($"Failed to deserialize {typeof(T).Name}: {s}");
    }
}
using System.Text.Json;

namespace Scriptube.API.Tests.Framework.Http;

public static class Polling
{
    public static async Task<JsonElement> WaitForBatchTerminalAsync(
        Func<Task<HttpResponseMessage>> getBatch,
        TimeSpan timeout,
        TimeSpan interval)
    {
        var start = DateTime.UtcNow;

        while (DateTime.UtcNow - start < timeout)
        {
            var res = await getBatch();
            var body = await res.Content.ReadAsStringAsync();

            // Keep generic: don’t assume schema; just look for common fields.
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement.Clone();

            var status = TryGetString(root, "status") ?? TryGetString(root, "state");

            if (!string.IsNullOrWhiteSpace(status))
            {
                var s = status.ToLowerInvariant();
                if (s is "completed" or "complete" or "done" or "finished" or "failed" or "cancelled" or "canceled" or "error")
                    return root;
            }

            await Task.Delay(interval);
        }

        throw new TimeoutException("Batch did not reach terminal state within timeout.");
    }

    private static string? TryGetString(JsonElement root, string name)
    {
        if (root.ValueKind != JsonValueKind.Object) return null;
        if (!root.TryGetProperty(name, out var p)) return null;
        return p.ValueKind == JsonValueKind.String ? p.GetString() : p.ToString();
    }
}
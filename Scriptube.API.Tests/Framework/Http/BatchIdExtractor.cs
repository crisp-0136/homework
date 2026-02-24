using System.Text.Json;

namespace Scriptube.API.Tests.Framework.Http;

public static class BatchIdExtractor
{
    public static string FromJson(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.TryGetProperty("batch_id", out var id) && id.ValueKind == JsonValueKind.String)
            return id.GetString()!;

        if (root.TryGetProperty("batchId", out var id2) && id2.ValueKind == JsonValueKind.String)
            return id2.GetString()!;

        throw new InvalidOperationException($"batch_id not found in response: {json}");
    }
}
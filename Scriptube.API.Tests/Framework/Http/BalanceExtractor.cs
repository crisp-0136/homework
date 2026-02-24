using System.Text.Json;

namespace Scriptube.API.Tests.Framework.Http;

public static class BalanceExtractor
{
    public static int FromJson(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.TryGetProperty("credits_balance", out var balance) && balance.ValueKind == JsonValueKind.Number)
            return balance.GetInt32();

        throw new InvalidOperationException($"balance not found in response: {json}");
    }
}
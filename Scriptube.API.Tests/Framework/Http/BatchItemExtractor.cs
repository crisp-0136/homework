using System.Text.Json;

public static class BatchItemExtractor
{
    public static string FirstItemId(string batchJson)
    {
        using var doc = JsonDocument.Parse(batchJson);
        var root = doc.RootElement;

        // common shapes: { items: [ { id: "..." } ] } or { items: [ { item_id: "..." } ] }
        if (!root.TryGetProperty("items", out var items) || items.ValueKind != JsonValueKind.Array || items.GetArrayLength() == 0)
            throw new InvalidOperationException($"No items[] found in batch response: {batchJson}");

        var first = items[0];

        if (first.TryGetProperty("id", out var id) && id.ValueKind == JsonValueKind.String)
            return id.GetString()!;

        if (first.TryGetProperty("item_id", out var id2) && id2.ValueKind == JsonValueKind.String)
            return id2.GetString()!;

        throw new InvalidOperationException($"No item id field found in first item: {first}");
    }
}
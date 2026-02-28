using System.Collections.Concurrent;

using Microsoft.AspNetCore.Http.Extensions;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var store = new ConcurrentQueue<HookEvent>();
const int Max = 50;

string AdminToken()
{
    var token = Environment.GetEnvironmentVariable("RECEIVER_ADMIN_TOKEN");
    if (string.IsNullOrWhiteSpace(token))
        throw new InvalidOperationException("RECEIVER_ADMIN_TOKEN not configured.");
    return token;
}

app.MapPost("/hook", async (HttpRequest req) =>
{
    using var reader = new StreamReader(req.Body);
    var body = await reader.ReadToEndAsync();

    var headers = req.Headers.ToDictionary(k => k.Key, v => v.Value.ToString());
    var evt = new HookEvent(
        ReceivedAtUtc: DateTime.UtcNow,
        Url: req.GetDisplayUrl(),
        Headers: headers,
        Body: body
    );

    store.Enqueue(evt);
    while (store.Count > Max && store.TryDequeue(out _)) { }

    return Results.Ok(new { ok = true });
});

app.MapGet("/last", (HttpRequest req) =>
{
    if (!IsAuthorized(req)) return Results.Unauthorized();

    if (!store.TryPeek(out var last))
        return Results.NotFound(new { message = "No events yet" });

    return Results.Ok(last);
});

app.MapGet("/all", (HttpRequest req) =>
{
    if (!IsAuthorized(req)) return Results.Unauthorized();
    return Results.Ok(store.ToArray());
});

app.MapPost("/clear", (HttpRequest req) =>
{
    if (!IsAuthorized(req)) return Results.Unauthorized();
    while (store.TryDequeue(out _)) { }
    return Results.Ok(new { cleared = true });
});

bool IsAuthorized(HttpRequest req)
{
    var token = req.Headers["X-Admin-Token"].ToString();
    return !string.IsNullOrWhiteSpace(token) && token == AdminToken();
}

app.Run();

public sealed record HookEvent(
    DateTime ReceivedAtUtc,
    string Url,
    Dictionary<string, string> Headers,
    string Body
);
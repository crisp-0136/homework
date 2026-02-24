// Framework/Models/TranscriptRequest.cs
namespace Scriptube.API.Tests.Framework.Models;

public sealed record TranscriptRequest
{
    public required List<string> Urls { get; init; }
    public bool Use_Byok { get; init; } = false;
    public bool Translate_To_English { get; init; } = false;
}


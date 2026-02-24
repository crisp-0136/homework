namespace Scriptube.API.Tests.Framework.Models;

public sealed record TranscriptsSubmitRequest
{
    public required List<string> Urls { get; init; }
    public bool TranslateToEnglish { get; init; } = false;
    public bool UseByok { get; init; } = false;
}
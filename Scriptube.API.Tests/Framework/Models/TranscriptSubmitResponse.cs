namespace Scriptube.API.Tests.Framework.Models;

public sealed record TranscriptSubmitResponse
{
    public required string Batch_Id { get; init; }
    public int Batch_Number { get; init; }
    public required string Status { get; init; }
    public int Url_Count { get; init; }
    public required string Message { get; init; }
    public string Key_Source { get; init; } = "system";
}
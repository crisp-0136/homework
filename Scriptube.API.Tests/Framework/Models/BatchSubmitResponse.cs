namespace Scriptube.API.Tests.Framework.Models;

public sealed record BatchSubmitResponse
{
    public required string BatchId { get; init; }
}
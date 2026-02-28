using FluentAssertions;
using Scriptube.Webhook.Tests.Models;

namespace Scriptube.Webhook.Tests.Assertions;

public static class ReceiverAsserts
{
    public static void MustContainBody(HookEvent evt)
    {
        evt.Body.Should().NotBeNullOrWhiteSpace("receiver must store request body");
        evt.Headers.Should().NotBeNull();
    }
}
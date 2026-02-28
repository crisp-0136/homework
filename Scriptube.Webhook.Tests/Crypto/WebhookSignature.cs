using Scriptube.Webhook.Tests.Models;

namespace Scriptube.Webhook.Tests.Crypto;

public static class WebhookSignature
{
    // Default header names; adjust once you confirm Scriptube’s exact scheme.
    public const string SignatureHeader = "X-Signature";
    public const string TimestampHeader = "X-Timestamp";

    // Common patterns:
    // - signature = HMAC_SHA256(secret, rawBody)
    // - signature = HMAC_SHA256(secret, $"{timestamp}.{rawBody}")
    public static bool Verify(
        HookEvent hookEvent,
        string secret,
        out string? failureReason,
        bool includeTimestampPrefix = true)
    {
        failureReason = null;

        if (!hookEvent.Headers.TryGetValue(SignatureHeader, out var sig) || string.IsNullOrWhiteSpace(sig))
        {
            failureReason = $"Missing {SignatureHeader}";
            return false;
        }

        hookEvent.Headers.TryGetValue(TimestampHeader, out var ts);

        var message = includeTimestampPrefix && !string.IsNullOrWhiteSpace(ts)
            ? $"{ts}.{hookEvent.Body}"
            : hookEvent.Body;

        // Accept hex or base64 signatures (normalize comparisons)
        var expectedHex = Hmac.Sha256Hex(secret, message);
        var expectedB64 = Hmac.Sha256Base64(secret, message);

        var sigNorm = sig.Trim();
        var ok = string.Equals(sigNorm, expectedHex, StringComparison.OrdinalIgnoreCase)
                 || string.Equals(sigNorm, expectedB64, StringComparison.Ordinal);

        if (!ok)
        {
            failureReason = "Signature mismatch";
        }

        return ok;
    }
}
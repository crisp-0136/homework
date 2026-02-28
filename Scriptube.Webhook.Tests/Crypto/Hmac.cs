using System.Security.Cryptography;
using System.Text;

namespace Scriptube.Webhook.Tests.Crypto;

public static class Hmac
{
    public static string Sha256Hex(string secret, string message)
    {
        var key = Encoding.UTF8.GetBytes(secret);
        var bytes = Encoding.UTF8.GetBytes(message);
        using var h = new HMACSHA256(key);
        var hash = h.ComputeHash(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public static string Sha256Base64(string secret, string message)
    {
        var key = Encoding.UTF8.GetBytes(secret);
        var bytes = Encoding.UTF8.GetBytes(message);
        using var h = new HMACSHA256(key);
        var hash = h.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
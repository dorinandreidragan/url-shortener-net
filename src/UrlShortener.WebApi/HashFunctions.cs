using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Components.Forms;

namespace UrlShortener.WebApi;

public static class HashFunctions
{
    public static string Sha256(string input, int hashLength = 6)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexStringLower(hash)[..hashLength];
    }

    public static string Base64(int hashLength = 6)
    {
        var input = Convert
            .ToBase64String(Guid.NewGuid().ToByteArray())
            .ToLowerInvariant();
        return Regex.Replace(input, "[/=+\\-]", "")[..6];
    }

    public static string RandomChars(int hashLength = 6)
    {
        const string charPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var bytes = new byte[hashLength];
        var key = new char[hashLength];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        for (int i = 0; i < hashLength; i++)
        {
            key[i] = charPool[bytes[i] % charPool.Length];
        }
        return new string(key);
    }
}
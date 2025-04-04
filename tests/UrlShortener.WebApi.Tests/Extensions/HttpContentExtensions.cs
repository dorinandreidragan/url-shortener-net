using System.Text;
using System.Text.Json;

namespace UrlShortener.WebApi.Tests.Extensions;

public static class HttpContentExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<T?> Deserialize<T>(this HttpResponseMessage response)
    {
        return JsonSerializer.Deserialize<T>(
            await response.Content.ReadAsStringAsync(),
            SerializerOptions);
    }

    public static HttpContent GetHttpContent<T>(this T obj) where T : class
    {
        return new StringContent(
            JsonSerializer.Serialize(obj),
            Encoding.UTF8, "application/json");
    }
}

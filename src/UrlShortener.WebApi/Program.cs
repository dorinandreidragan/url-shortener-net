using System.Collections.Concurrent;

using UrlShortener.WebApi;

var builder = WebApplication.CreateBuilder(args);
var baseUrl = builder.Configuration["BaseUrl"]
    ?? throw new InvalidOperationException("BaseUrl not configured");

var app = builder.Build();
app.Urls.Add(baseUrl);

var urls = new ConcurrentDictionary<string, string>();

app.MapPost("/shorten", (UrlShortenRequest request) =>
{

    string shortKey = urls.FirstOrDefault(x => x.Value == request.OriginalUrl).Key;
    if (string.IsNullOrEmpty(shortKey))
    {
        shortKey = HashFunctions.Base64();
        urls[shortKey] = request.OriginalUrl;
    }

    return Results.Ok(new UrlShortenResponse($"{baseUrl.TrimEnd('/')}/{shortKey}"));
});


app.MapGet("/{shortKey}", (string shortKey) =>
{
    if (urls.TryGetValue(shortKey, out var originalUrl))
    {
        return Results.Redirect(originalUrl);
    }
    return Results.NotFound();
});

app.Run();

public record UrlShortenRequest(string OriginalUrl);
public record UrlShortenResponse(string ShortUrl);

public partial class Program { }
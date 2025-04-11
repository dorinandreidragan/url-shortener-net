using System.Collections.Concurrent;

using Microsoft.EntityFrameworkCore;

using UrlShortener.WebApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UrlShortenerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var baseUrl = builder.Configuration["BaseUrl"]
    ?? throw new InvalidOperationException("BaseUrl not configured");

var app = builder.Build();
app.Urls.Add(baseUrl);

var urls = new ConcurrentDictionary<string, string>();

app.MapPost("/shorten", async (UrlShortenRequest request, UrlShortenerDbContext dbContext) =>
{
    if (string.IsNullOrEmpty(request.OriginalUrl))
    {
        return Results.BadRequest();
    }

    string shortKey;
    var existingMapping = await dbContext.UrlMappings
        .FirstOrDefaultAsync(x => x.OriginalUrl == request.OriginalUrl);

    if (existingMapping == null)
    {
        shortKey = HashFunctions.Base64();

        // add the new short key in the database.
        var urlMapping = new UrlMapping
        {
            OriginalUrl = request.OriginalUrl,
            ShortKey = shortKey
        };
        dbContext.UrlMappings.Add(urlMapping);
        await dbContext.SaveChangesAsync();
    }
    else
    {
        shortKey = existingMapping.ShortKey;
    }

    return Results.Ok(new UrlShortenResponse($"{baseUrl.TrimEnd('/')}/{shortKey}"));
});


app.MapGet("/{shortKey}", async (string shortKey, UrlShortenerDbContext dbContext) =>
{
    var existingMapping = await dbContext.UrlMappings
        .FirstOrDefaultAsync(x => x.ShortKey == shortKey);

    if (existingMapping != null)
    {
        return Results.Redirect(existingMapping.OriginalUrl);
    }
    return Results.NotFound();
});

app.Run();

public record UrlShortenRequest(string OriginalUrl);
public record UrlShortenResponse(string ShortUrl);

public partial class Program { }
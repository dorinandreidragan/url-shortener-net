namespace UrlShortener.WebApi;

public record UrlMapping
{
    public int Id { get; init; }
    public required string OriginalUrl { get; init; }
    public required string ShortKey { get; init; }
}

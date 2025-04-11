using System.Net;
using System.Text.Json;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using UrlShortener.WebApi.Tests.Extensions;
using UrlShortener.WebApi.Tests.Factories;
using UrlShortener.WebApi.Tests.Fixtures;

namespace UrlShortener.WebApi.Tests;

[Collection(nameof(IntegrationTestsCollection))]
public class UrlShortenerTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory factory;
    private readonly HttpClient client;
    private readonly string baseUrl;
    private readonly UrlShortenerDbContext dbContext;

    public UrlShortenerTests(PosgreSqlContainerFixture fixture)
    {
        this.factory = new CustomWebApplicationFactory(fixture.Postgres.GetConnectionString());
        this.client = this.factory.CreateClient();
        this.baseUrl = this.client.BaseAddress!.OriginalString;

        var scope = this.factory.Services.CreateScope();
        dbContext = scope.ServiceProvider.GetRequiredService<UrlShortenerDbContext>();
    }

    public async Task InitializeAsync()
    {
        // Make sure that the db is clean every time we run a test
        // so that the tests run in isolation.
        dbContext.UrlMappings.RemoveRange(dbContext.UrlMappings);
        await dbContext.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        this.client.Dispose();
        this.factory.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Shorten_Should_Return_Short_Url_For_Valid_Input()
    {
        var content = new UrlShortenRequest("https://www.example.com").GetHttpContent();

        var response = await this.client.PostAsync("/shorten", content);

        response.EnsureSuccessStatusCode();
        var result = await response.Deserialize<UrlShortenResponse>();
        result.Should().NotBeNull();
        result.ShortUrl.Should().StartWith(this.baseUrl);
    }

    [Fact]
    public async Task Shorten_Should_Return_Same_Short_Url_When_Called_Twice_With_Same_Input()
    {
        var content = new UrlShortenRequest("https://www.example.com").GetHttpContent();
        await this.client.PostAsync("/shorten", content);

        var response = await this.client.PostAsync("/shorten", content);

        response.EnsureSuccessStatusCode();
        var result = await response.Deserialize<UrlShortenResponse>();
        result.Should().NotBeNull();
        result.ShortUrl.Should().StartWith(this.baseUrl);
    }

    [Fact]
    public async Task Redirect_Should_Return_Not_Found_For_Invalid_ShortKey()
    {
        var response = await this.client.GetAsync(string.Empty);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Shorten_Returns_BadRequest_For_Invalid_Url()
    {
        var content = new UrlShortenRequest(string.Empty).GetHttpContent();

        var response = await this.client.PostAsync("/shorten", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
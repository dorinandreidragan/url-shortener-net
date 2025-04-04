using System.Text.Json;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

using UrlShortener.WebApi.Tests.Extensions;

namespace UrlShortener.WebApi.Tests;

public class UrlShortenerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;
    private readonly string baseUrl;

    public UrlShortenerTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.Test.json", optional: true, reloadOnChange: true)
            .Build();
        this.baseUrl = config["BaseUrl"] ?? throw new InvalidOperationException("BaseUrl not configured.");
    }

    [Fact]
    public async Task Shorten_Returns_Short_Url()
    {
        var content = new UrlShortenRequest("https://www.example.com").GetHttpContent();

        var response = await this.client.PostAsync("/shorten", content);

        response.EnsureSuccessStatusCode();
        var result = await response.Deserialize<UrlShortenResponse>();
        result.Should().NotBeNull();
        result.ShortUrl.Should().StartWith(this.baseUrl);
    }

    [Fact]
    public async Task Shorten_Returns_Short_Url_When_Called_Twice()
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
    public async Task Redirect_Returns_Not_Found_When_Wrong_ShortKey()
    {
        var response = await this.client.GetAsync("/invalid_short_key");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}
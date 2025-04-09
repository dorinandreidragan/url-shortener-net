using System.Text.Json;

using DotNet.Testcontainers.Builders;

using FluentAssertions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Npgsql;

using Testcontainers.PostgreSql;

using UrlShortener.WebApi.Tests.Extensions;

namespace UrlShortener.WebApi.Tests;

public class PostgreSqlTestContainer : IAsyncLifetime
{
    public readonly PostgreSqlContainer Container;

    public PostgreSqlTestContainer()
    {
        // Getting and parsing the connection string so that we can configure the container via the
        // appsettings.Test.json file.
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.Test.json", optional: true, reloadOnChange: true)
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

        this.Container = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase(connectionStringBuilder.Database)
            .WithUsername(connectionStringBuilder.Username)
            .WithPassword(connectionStringBuilder.Password)
            .WithPortBinding(connectionStringBuilder.Port, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready"))
            .Build();
    }

    public async Task InitializeAsync()
    {
        await this.Container.StartAsync();

        var options = new DbContextOptionsBuilder<UrlShortenerDbContext>()
            .UseNpgsql(this.Container.GetConnectionString())
            .Options;

        using var context = new UrlShortenerDbContext(options);
        await context.Database.MigrateAsync();
    }

    public Task DisposeAsync()
    {
        return this.Container.DisposeAsync().AsTask();
    }
}

public sealed class TestsWebApplicationFactory(Action<IServiceCollection> configureServices)
    : WebApplicationFactory<Program>
{
    private readonly Action<IServiceCollection> configureServices = configureServices;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(this.configureServices);
    }
}

public static class PostgreSqlServicesConfigurator
{
    public static Action<IServiceCollection> Configure()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.Test.json", optional: true, reloadOnChange: true)
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");

        return services =>
        {
            services.AddDbContext<UrlShortenerDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        };

    }
}

public class UrlShortenerTests : IClassFixture<PostgreSqlTestContainer>, IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;
    private readonly string baseUrl;

    public UrlShortenerTests(PostgreSqlTestContainer fixture)
    {
        var services = PostgreSqlServicesConfigurator.Configure();
        this.factory = new TestsWebApplicationFactory(services);
        this.client = this.factory.CreateClient();
        this.baseUrl = this.client.BaseAddress!.OriginalString;
    }

    public void Dispose()
    {
        this.factory.Dispose();
        GC.SuppressFinalize(this);
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
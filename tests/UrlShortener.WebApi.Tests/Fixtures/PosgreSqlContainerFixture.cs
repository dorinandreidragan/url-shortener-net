using Microsoft.EntityFrameworkCore;

using Testcontainers.PostgreSql;

namespace UrlShortener.WebApi.Tests.Fixtures;

public class PosgreSqlContainerFixture : IAsyncLifetime
{
    public PostgreSqlContainer Postgres { get; private set; }

    public PosgreSqlContainerFixture()
    {
        Postgres = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await Postgres.StartAsync();

        // Apply EF core migrations after Progres is ready
        var options = new DbContextOptionsBuilder<UrlShortenerDbContext>()
            .UseNpgsql(Postgres.GetConnectionString())
            .Options;

        using var context = new UrlShortenerDbContext(options);
        await context.Database.MigrateAsync();
    }

    public Task DisposeAsync()
    {
        return Postgres.DisposeAsync().AsTask();
    }
}

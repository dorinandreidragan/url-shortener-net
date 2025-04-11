using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace UrlShortener.WebApi.Tests.Factories;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string postgreSqlConnectionString;

    public CustomWebApplicationFactory(string postgreSqlConnectionString)
    {
        this.postgreSqlConnectionString = postgreSqlConnectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddDbContext<UrlShortenerDbContext>(options =>
            {
                options.UseNpgsql(postgreSqlConnectionString);
            });
        });
    }
}

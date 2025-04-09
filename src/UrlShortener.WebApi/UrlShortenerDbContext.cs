using Microsoft.EntityFrameworkCore;

namespace UrlShortener.WebApi;

public class UrlShortenerDbContext : DbContext
{
    public DbSet<UrlMapping> UrlMappings { get; set; }

    public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UrlMapping>()
            .HasKey(u => u.Id);
        modelBuilder.Entity<UrlMapping>()
            .HasIndex(u => u.ShortKey)
            .IsUnique();
    }
}

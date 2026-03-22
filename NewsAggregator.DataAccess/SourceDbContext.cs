using Microsoft.EntityFrameworkCore;
using NewsAggregator.Domain.Models;

namespace NewsAggregator.DataAccess;

public class SourceDbContext(DbContextOptions<SourceDbContext> options) : DbContext(options)
{
    public DbSet<Source> Sources => Set<Source>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SourceDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
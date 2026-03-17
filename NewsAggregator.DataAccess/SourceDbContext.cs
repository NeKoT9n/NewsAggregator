using Microsoft.EntityFrameworkCore;
using NewsAggregator.Domain.Models;

namespace NewsAggregator.DataAccess;

public class SourceDbContext : DbContext
{
    public DbSet<Source> Sources { get; set; }
}
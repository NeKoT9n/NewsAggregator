using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsAggregator.Domain.Models;

namespace NewsAggregator.DataAccess.Configurations;

public class SourceScraperConfigConfiguration : IEntityTypeConfiguration<SourceScraperConfig>
{
    public void Configure(EntityTypeBuilder<SourceScraperConfig> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ArticleContentSelector)
            .HasMaxLength(256);

        builder.Property(x => x.IgnoreSelector)
            .HasMaxLength(256);

        builder.Property(x => x.ImageSelector)
            .HasMaxLength(256);

        builder.HasIndex(x => x.SourceId)
            .IsUnique();
    }
}
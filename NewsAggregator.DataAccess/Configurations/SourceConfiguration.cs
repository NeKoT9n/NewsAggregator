using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsAggregator.Domain.Models;

namespace NewsAggregator.DataAccess.Configurations;

public class SourceConfiguration : IEntityTypeConfiguration<Source>
{
    public void Configure(EntityTypeBuilder<Source> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.BaseUrl)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.RssUrl)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(x => x.Language)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(x => x.RssUrl)
            .IsUnique();

        builder.HasOne(x => x.ScraperConfig)
            .WithOne(x => x.Source)
            .HasForeignKey<SourceScraperConfig>(x => x.SourceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
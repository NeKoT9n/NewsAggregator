using Microsoft.EntityFrameworkCore;
using NewsAggregator.DataAccess;
using NewsAggregator.Domain.Models;
using NewsAggregator.Infostructure.Dto;
using Shared.Common.Validation;


namespace NewsAggregator.Infostructure.Services;

public class SourceService(SourceDbContext context, SourceTester tester)
{
    public async Task<Result<long, Error>> AddSourceAsync(AddSourceCommand command, CancellationToken ct)
    {
        var exists = await context.Sources
            .AnyAsync(x => x.RssUrl == command.RssUrl, ct);
        
        if (exists)
            return Errors.General.Validation("Источник с таким RSS URL уже зарегистрирован");

        var result = await tester.TestSourceAsync(command, ct);

        if (result.IsFailure)
            return result.Error;
                
        var source = new Source
        {
            Name = command.Name,
            BaseUrl = command.BaseUrl,
            RssUrl = command.RssUrl,
            Language = command.Language,
            IsActive = command.IsActive,

            ScraperConfig = command.ScraperConfig != null
                ? new SourceScraperConfig
                {
                    ArticleContentSelector = command.ScraperConfig.ArticleContentSelector,
                    IgnoreSelector = command.ScraperConfig.IgnoreSelector,
                    ImageSelector = command.ScraperConfig.ImageSelector
                }
                : null
        };
        
        context.Sources.Add(source);
        await context.SaveChangesAsync(ct);

        return source.Id;
    }

    public async Task<List<Source>> GetAllSourcesAsync(CancellationToken ct)
    {
        return await context.Sources.ToListAsync(ct);
    }

    public async Task<Result<long, Error>> DeleteSourceAsync(long id, CancellationToken ct)
    {
        var source = await context.Sources.FindAsync([id], ct);

        if (source == null) 
            return Errors.General.NotFound("Source");

        context.Sources.Remove(source);
        await context.SaveChangesAsync(ct);

        return id;
    }

    public async Task<Result<bool, Error>> ToggleSourceStatusAsync(long id, CancellationToken ct)
    {
        var source = await context.Sources.FindAsync([id], ct);
        if (source == null) return Errors.General.NotFound(id);

        source.IsActive = !source.IsActive;

        await context.SaveChangesAsync(ct);

        return true;
    }
}
using NewsAggregator.Domain.Models;
using NewsAggregator.Infostructure.Dto;
using NewsAggregator.Infostructure.Services.ArticleProviders.RssScrapper;
using Shared.Common.Validation;

namespace NewsAggregator.Infostructure.Services;

public class SourceTester(RssParser parser, ArticleScraper scraper)
{
    public async Task<Result<bool, Error>> TestSourceAsync(AddSourceCommand source, CancellationToken ct)
    {
        try
        {
            var items = await parser.ParseAsync(source.RssUrl);

            if (items.Count == 0)
                return Errors.Source.TestFailed("RSS invalid format");

            var firstItemUrl = items.First();

            var configToAdd = source.ScraperConfig;

            if (configToAdd == null)
                return Errors.Source.TestFailed("Scrap config is empty");


            var config = new SourceScraperConfig()
            {
                ArticleContentSelector = configToAdd.ArticleContentSelector,
                IgnoreSelector = configToAdd.IgnoreSelector,
                ImageSelector = configToAdd.ImageSelector
            };

            var result = await scraper.ScrapeAsync(firstItemUrl, config);

            if (result.IsFailure)
                return Errors.Source.TestFailed(result.Error.Message);
            
            return true;
        }
        catch (Exception ex)
        {
            return Errors.Source.TestFailed(ex.Message);
        }
    }
}
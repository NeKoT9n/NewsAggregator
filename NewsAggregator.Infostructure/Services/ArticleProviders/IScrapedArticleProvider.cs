using NewsAggregator.Domain.Models;
using Shared.Models.Messages;

namespace NewsAggregator.Infostructure.Services.ArticleProviders;

public interface IScrapedArticleProvider
{
    public Task<IReadOnlyList<ScrapedArticleDto>> GetArticlesAsync(Source source);
}




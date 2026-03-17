namespace NewsAggregator.Infostructure.Dto;

public record ScrappedArticle(
    string Title, 
    string FullText, 
    string? MainImageUrl, 
    string OriginalUrl, 
    DateTime PublishDate
);
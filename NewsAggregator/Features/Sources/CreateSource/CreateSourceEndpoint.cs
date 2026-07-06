using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NewsAggregator.DataAccess;
using NewsAggregator.Domain.Models;
using NewsAggregator.Infostructure.Dto;
using NewsAggregator.Infostructure.Services;
using SourceEntity = NewsAggregator.Domain.Models.Source;

namespace NewsAggregator.Features.Sources.CreateSource;

public class CreateSourceEndpoint(SourceService sourceService) : Endpoint<CreateSourceRequest, long>
{
    public override void Configure()
    {
        Post("api/source/add");
        Roles("Admin");
        Summary(s => {
            s.Summary = "Добавить новый RSS-источник";
            s.Description = "Создает запись в БД агрегатора для последующего опроса воркером";
        });
    }

        public override async Task HandleAsync(CreateSourceRequest request, CancellationToken ct)
        {

            var command = new AddSourceCommand()
            {
                Name = request.Name,
                BaseUrl = request.BaseUrl,
                RssUrl = request.RssUrl,
                Language = request.Language,
                IsActive = request.IsActive,

                ScraperConfig = new AddScraperConfigCommand()
                {
                    ArticleContentSelector = request.ScraperConfig.ArticleContentSelector,
                    IgnoreSelector = request.ScraperConfig.IgnoreSelector,
                    ImageSelector = request.ScraperConfig.ImageSelector
                }

            };
            
            var result = await sourceService.AddSourceAsync(command, ct);

            if (result.IsFailure)
            {
                AddError(result.Error.Message);
                await Send.ErrorsAsync(400, ct);
                return;
            }
            
            await Send.OkAsync(result.Value, ct);
        }
}
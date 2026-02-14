using Shared.Models.Messages;

namespace NewsAggregator.Services;

public interface IRawNewsProvider
{
    public Task<IReadOnlyList<RawNewsScraped>> GetNewsAsync();
}




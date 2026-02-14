using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using NewsAggregator.Options;
using StackExchange.Redis;

namespace NewsAggregator.Services;

public class NewsCacheService
{
    private readonly IDatabase _db;
    private readonly RedisOptions _options;

    public NewsCacheService(IOptions<RedisOptions> options)
    {
        _options = options.Value;
        
        var redis = ConnectionMultiplexer.Connect(_options.ConnectionString);
        _db = redis.GetDatabase();
    }

    public async Task<bool> IsAlreadyProcessed(string url)
    {
        
        return await _db.KeyExistsAsync(GetKey(url));
    }

    public async Task MarkAsProcessed(string url)
    {
        await _db.StringSetAsync(GetKey(url), "true",
            TimeSpan.FromDays(_options.CacheExpirationDays));
    }

    private string GetKey(string url)
    {
        var normalizedUrl = url.Trim().ToLowerInvariant();
        var inputBytes = Encoding.UTF8.GetBytes(normalizedUrl);
            
        var hash = MD5.HashData(inputBytes);
        var hashString = Convert.ToHexString(hash);
        
        return $"{_options.InstanceName}:processed:{hashString}";
    }
}
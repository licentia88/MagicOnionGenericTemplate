using Benutomo;
using MagicT.Redis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace MagicT.Redis.Services;

/// <summary>
/// Provides rate limiting functionality using Redis as the data store.
/// </summary>
[AutomaticDisposeImpl]
public sealed partial class RateLimiterService:IDisposable,IAsyncDisposable
{
    [EnableAutomaticDispose]
    private readonly MagicTRedisDatabase _magicTRedisDatabase;
    
    [EnableAutomaticDispose]
    private readonly RateLimiterConfig _rateLimiterConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimiterService"/> class using dependency injection.
    /// </summary>
    /// <param name="provider">The service provider used for dependency injection.</param>
    /// <param name="configuration">The configuration settings.</param>
    public RateLimiterService(IServiceProvider provider, IConfiguration configuration)
    {
        _magicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();
        _rateLimiterConfig = configuration.GetSection(nameof(RateLimiterConfig)).Get<RateLimiterConfig>();
    }

    ~RateLimiterService()
    {
        Dispose();
        GC.WaitForPendingFinalizers();
    }
    /// <summary>
    /// Checks if the client has exceeded the rate limit and increments the request count.
    /// </summary>
    /// <param name="clientId">The unique identifier of the client.</param>
    /// <returns>Returns <c>true</c> if the client is within the rate limit; otherwise, <c>false</c>.</returns>
    public bool CheckRateLimit(string clientId)
    {
        var redisKey = $"RateLimit:{clientId}";

        var script = @"
            local currentCount = redis.call('GET', KEYS[1])
            if not currentCount then
                currentCount = 0
            end
            if tonumber(currentCount) >= tonumber(ARGV[1]) then
                return 0
            else
                redis.call('INCR', KEYS[1])
                redis.call('EXPIRE', KEYS[1], tonumber(ARGV[2]))
                return 1
            end";

        var result = (int)_magicTRedisDatabase.MagicTRedisDb.ScriptEvaluate(script, new RedisKey[] { redisKey }, new RedisValue[] { _rateLimiterConfig.RateLimit, _rateLimiterConfig.PerSecond });

        return result == 1;
    }
}
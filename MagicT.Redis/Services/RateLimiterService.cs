using MagicT.Redis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace MagicT.Redis.Services;

/// <summary>
///     Provides rate limiting functionality using Redis as the data store.
/// </summary>
public sealed class RateLimiterService
{
    private readonly MagicTRedisDatabase MagicTRedisDatabase;

    private readonly RateLimiterConfig RateLimiterConfig;

    /// <summary>
    ///     Initializes a new instance of the RateLimiter class using dependency injection.
    /// </summary>
    /// <param name="provider">The service provider used for dependency injection.</param>
    public RateLimiterService(IServiceProvider provider, IConfiguration configuration)
    {
        MagicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();
        RateLimiterConfig = configuration.GetSection(nameof(RateLimiterConfig)).Get<RateLimiterConfig>();
    }
    public IServiceProvider Provider { get; set; }

    /// <summary>
    ///     Checks if the client has exceeded the rate limit and increments the request count.
    /// </summary>
    /// <param name="clientId">The unique identifier of the client.</param>
    /// <returns>Returns true if the client is within the rate limit; otherwise, false.</returns>
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

        var result = (int)MagicTRedisDatabase.MagicTRedisDb.ScriptEvaluate(script, new RedisKey[] { redisKey }, new RedisValue[] { RateLimiterConfig.RateLimit, RateLimiterConfig.PerSecond });

        return result == 1;
    }
    //public bool CheckRateLimit(string clientId)
    //{
    //    // Generate the Redis key for the client's rate limit.
    //    var redisKey = $"RateLimit:{clientId}";

    //    // Get the current count of requests for the client from Redis.
    //    var currentCount = (int)MagicTRedisDatabase.MagicTRedisDb.StringGet(redisKey);

    //    // Check if the client has exceeded the rate limit.
    //    if (currentCount >= RateLimiterConfig.RateLimit) return false;

    //    // Increment the request count for the client within the current time window.
    //    MagicTRedisDatabase.MagicTRedisDb.StringIncrement(redisKey);

    //    // Set the expiration time for the Redis key to match the time window.
    //    MagicTRedisDatabase.MagicTRedisDb.KeyExpire(redisKey, TimeSpan.FromSeconds(RateLimiterConfig.PerSecond));

    //    return true;
    //}
}
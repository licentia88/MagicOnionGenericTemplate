using StackExchange.Redis;

public class RateLimitHelper
{
    private readonly IDatabase _redisDb;

    public RateLimitHelper()
    {
        // Get a reference to the Redis server.
        _redisDb = RedisHelper.Connection.GetDatabase();
    }

    public bool CheckRateLimit(Guid clientId, int requestLimit, TimeSpan timeWindow)
    {
        // Generate the Redis key for the client's rate limit.
        string redisKey = $"RateLimit:{clientId}";

        // Get the current count of requests for the client from Redis.
        int currentCount = (int)_redisDb.StringGet(redisKey);

        // Check if the client has exceeded the rate limit.
        if (currentCount >= requestLimit)
        {
            return false;
        }

        // Increment the request count for the client within the current time window.
        // Note that the INCR command in Redis is atomic, ensuring thread safety.
        // The time window will be enforced by the Redis key expiration.
        _redisDb.StringIncrement(redisKey);

        // Set the expiration time for the Redis key to match the time window.
        _redisDb.KeyExpire(redisKey, timeWindow);

        return true;
    }
}



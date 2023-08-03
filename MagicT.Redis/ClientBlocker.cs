using MagicT.Redis.Options;
using Microsoft.Extensions.DependencyInjection;
 
namespace MagicT.Redis;

public class ClientBlocker
{
    private readonly MagicTRedisDatabase MagicTRedisDatabase;

    private readonly RateLimiterConfig RateLimiterConfig;


    public ClientBlocker(IServiceProvider provider)
    {
        MagicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();
        RateLimiterConfig = provider.GetService<RateLimiterConfig>();
    }

    public bool IsSoftBlocked(string clientId)
    {
        var redisKey = $"SoftBlock:{clientId}";
        var requestCount = (int)MagicTRedisDatabase.MagicTRedisDb.StringGet(redisKey);
        return requestCount >= RateLimiterConfig.SoftBlockCount;
    }


    public bool IsHardBlocked(string clientId)
    {
        // Implement hard block client logic based on specific rules.
        // For example, you can check if the client IP is in a hard block list or has violated critical rules.
        // You may use Redis sets to store a list of hard-blocked clients' IDs.

        const string redisKey = "HardBlockList";
        return MagicTRedisDatabase.MagicTRedisDb.SetContains(redisKey, clientId);
    }
 
    public void AddSoftBlock(string clientId)
    {
        var softBlockCount = GetSoftBlockCount(clientId) + 1;
        var redisKey = GetSoftBlockCountRedisKey(clientId);

        // If soft block count exceeds the limit, block the client for a lifetime.
        if (softBlockCount > RateLimiterConfig.SoftBlockCount)
            AddHardBlock(clientId);
        else
        {
            // Increment the soft block count and set the expiration time.
            MagicTRedisDatabase.MagicTRedisDb.StringSet(redisKey, softBlockCount, TimeSpan.FromHours(RateLimiterConfig.SoftBlockDurationMinutes));
        }
    }

    private void AddHardBlock(string clientId)
    {
        const string redisKey = "HardBlockList";
        MagicTRedisDatabase.MagicTRedisDb.SetAdd(redisKey, clientId);
    }

    public void RemoveBlock(string clientId)
    {
        var redisKey = GetSoftBlockCountRedisKey(clientId);
        MagicTRedisDatabase.MagicTRedisDb.KeyDelete(redisKey);

        const string hardBlockKey = "HardBlockList";
        MagicTRedisDatabase.MagicTRedisDb.SetRemove(hardBlockKey, clientId);
    }

    private int GetSoftBlockCount(string clientId)
    {
        var redisKey = GetSoftBlockCountRedisKey(clientId);
        return (int)MagicTRedisDatabase.MagicTRedisDb.StringGet(redisKey);
    }

    private string GetSoftBlockCountRedisKey(string clientId)
    {
        return $"SoftBlockCount:{clientId}";
    }

}

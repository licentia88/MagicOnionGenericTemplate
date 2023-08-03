using MagicT.Redis.Options;
using Microsoft.Extensions.DependencyInjection;
 
namespace MagicT.Redis;

public class ClientBlocker
{
    private readonly MagicTRedisDatabase MagicTRedisDatabase;

    private readonly RateLimiterConfig RateLimiterConfig;

    public IServiceProvider Provider { get; set; }
   
 
    public ClientBlocker(IServiceProvider provider)
    {
        MagicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();
        RateLimiterConfig = provider.GetService<RateLimiterConfig>();
    }

    public bool IsSoftBlocked(string clientId)
    {
        string redisKey = $"SoftBlock:{clientId}";
        int requestCount = (int)MagicTRedisDatabase.MagicTRedisDb.StringGet(redisKey);
        return requestCount >= RateLimiterConfig.SoftBlockCount;
    }


    public bool IsHardBlocked(string clientId)
    {
        // Implement hard block client logic based on specific rules.
        // For example, you can check if the client IP is in a hard block list or has violated critical rules.
        // You may use Redis sets to store a list of hard-blocked clients' IDs.

        string redisKey = "HardBlockList";
        return MagicTRedisDatabase.MagicTRedisDb.SetContains(redisKey, clientId.ToString());
    }
 
    public void AddSoftBlock(string clientId)
    {
        int softBlockCount = GetSoftBlockCount(clientId) + 1;
        string redisKey = GetSoftBlockCountRedisKey(clientId);

        // If soft block count exceeds the limit, block the client for a lifetime.
        if (softBlockCount > RateLimiterConfig.SoftBlockCount)
            AddHardBlock(clientId);
        else
        {
            // Increment the soft block count and set the expiration time.
            MagicTRedisDatabase.MagicTRedisDb.StringSet(redisKey, softBlockCount, TimeSpan.FromHours(RateLimiterConfig.SoftBlockDurationMinutes));
        }
    }

    public void AddHardBlock(string clientId)
    {
        string redisKey = "HardBlockList";
        MagicTRedisDatabase.MagicTRedisDb.SetAdd(redisKey, clientId.ToString());
    }

    public void RemoveBlock(string clientId)
    {
        string redisKey = GetSoftBlockCountRedisKey(clientId);
        MagicTRedisDatabase.MagicTRedisDb.KeyDelete(redisKey);

        string hardBlockKey = "HardBlockList";
        MagicTRedisDatabase.MagicTRedisDb.SetRemove(hardBlockKey, clientId.ToString());
    }

    private int GetSoftBlockCount(string clientId)
    {
        string redisKey = GetSoftBlockCountRedisKey(clientId);
        return (int)MagicTRedisDatabase.MagicTRedisDb.StringGet(redisKey);
    }

    private string GetSoftBlockCountRedisKey(string clientId)
    {
        return $"SoftBlockCount:{clientId}";
    }

}

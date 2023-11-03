using MagicT.Redis.Options;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Redis.Services;

/// <summary>
/// Service for managing client blocking based on soft and hard block rules.
/// </summary>
public sealed class ClientBlockerService
{
    private readonly MagicTRedisDatabase MagicTRedisDatabase;
    private readonly RateLimiterConfig RateLimiterConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientBlockerService"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public ClientBlockerService(IServiceProvider provider)
    {
        MagicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();
        RateLimiterConfig = provider.GetService<RateLimiterConfig>();
    }

    /// <summary>
    /// Checks if a client is soft-blocked.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <returns><c>true</c> if the client is soft-blocked; otherwise, <c>false</c>.</returns>
    public bool IsSoftBlocked(string clientId)
    {
        var softBlockCount = GetSoftBlockCount(clientId);
        return softBlockCount > 0;
    }

    /// <summary>
    /// Checks if a client is hard-blocked.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <returns><c>true</c> if the client is hard-blocked; otherwise, <c>false</c>.</returns>
    public bool IsHardBlocked(string clientId)
    {
        // Implement hard block client logic based on specific rules.
        // For example, you can check if the client IP is in a hard block list or has violated critical rules.
        // You may use Redis sets to store a list of hard-blocked clients' IDs.

        const string redisKey = "HardBlockList";
        return MagicTRedisDatabase.MagicTRedisDb.SetContains(redisKey, clientId);
    }

    /// <summary>
    /// Adds a soft block for a client.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    public void AddSoftBlock(string clientId)
    {
        var softBlockCount = GetSoftBlockCount(clientId) + 1;
        var redisKey = GetSoftBlockCountRedisKey(clientId);

        // If soft block count exceeds the limit, block the client for a lifetime.
        if (softBlockCount > RateLimiterConfig.SoftBlockCount)
            AddHardBlock(clientId);
        else
            // Increment the soft block count and set the expiration time.
            MagicTRedisDatabase.MagicTRedisDb.StringSet(redisKey, softBlockCount,
                TimeSpan.FromMinutes(RateLimiterConfig.SoftBlockDurationMinutes));
    }

    /// <summary>
    /// Adds a hard block for a client.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    public void AddHardBlock(string clientId)
    {
        const string redisKey = "HardBlockList";
        MagicTRedisDatabase.MagicTRedisDb.SetAdd(redisKey, clientId);
        // Adds the specified client identifier to the hard block list.
    }

    /// <summary>
    /// Removes a block for a client.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
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
        return (int) MagicTRedisDatabase.MagicTRedisDb.StringGet(redisKey);
    }

    private string GetSoftBlockCountRedisKey(string clientId)
    {
        return $"SoftBlockCount:{clientId}";
    }
}
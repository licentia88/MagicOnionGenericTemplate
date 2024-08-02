using MagicT.Redis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;

namespace MagicT.Redis.Services
{
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
        /// <param name="configuration">The configuration settings.</param>
        public ClientBlockerService(IServiceProvider provider, IConfiguration configuration)
        {
            MagicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();
            RateLimiterConfig = configuration.GetSection(nameof(RateLimiterConfig)).Get<RateLimiterConfig>();
        }

        /// <summary>
        /// Checks if a client is soft-blocked.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns><c>true</c> if the client is soft-blocked; otherwise, <c>false</c>.</returns>
        public bool IsSoftBlocked(string clientId)
        {
            var softBlockDuration = GetSoftBlockDuration(clientId);
            return softBlockDuration > 0;
        }

        /// <summary>
        /// Checks if a client is hard-blocked.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns><c>true</c> if the client is hard-blocked; otherwise, <c>false</c>.</returns>
        public bool IsHardBlocked(string clientId)
        {
            const string redisKey = "HardBlockList";
            return MagicTRedisDatabase.MagicTRedisDb.SetContains(redisKey, clientId);
        }

        /// <summary>
        /// Adds a soft block for a client using Luascript for less database roundrips and performance Improvement
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        public void AddSoftBlock(string clientId)
        {
            var softBlockCountKey = GetSoftBlockCountRedisKey(clientId);
            var softBlockDurationKey = GetSoftBlockDurationRedisKey(clientId);

            var softBlockCount = GetSoftBlockCount(clientId) + 1;

            // Log the current soft block count
            Console.WriteLine($"Adding soft block for {clientId}. Current soft block count is {softBlockCount}, new count will be {softBlockCount}.");

            // If soft block count exceeds the limit, add a hard block
            if (softBlockCount > RateLimiterConfig.SoftBlockCount)
            {
                AddHardBlock(clientId);
            }
            else
            {
                // Increment the soft block count (without expiration)
                MagicTRedisDatabase.MagicTRedisDb.StringSet(softBlockCountKey, softBlockCount);

                // Set the soft block duration with expiration
                MagicTRedisDatabase.MagicTRedisDb.StringSet(softBlockDurationKey, 1, TimeSpan.FromMinutes(RateLimiterConfig.SoftBlockDurationMinutes));
            }

            // Log the action taken
            Console.WriteLine($"Soft block added for {clientId}. Current soft block count is now {softBlockCount}.");
        }
        /// <summary>
        /// Obsolate
        /// </summary>
        /// <param name="clientId"></param>
        ///
        //[Obsolete]
        //public void AddSoftBlock(string clientId)
        //{
        //    var softBlockCountKey = GetSoftBlockCountRedisKey(clientId);
        //    var softBlockDurationKey = GetSoftBlockDurationRedisKey(clientId);

        //    var softBlockCount = GetSoftBlockCount(clientId) + 1;

        //    // Log the current soft block count
        //    Console.WriteLine($"Adding soft block for {clientId}. Current soft block count is {softBlockCount }, new count will be {softBlockCount}.");

        //    // If soft block count exceeds the limit, add a hard block
        //    if (softBlockCount > RateLimiterConfig.SoftBlockCount)
        //    {
        //        AddHardBlock(clientId);
        //    }
        //    else
        //    {
        //        // Increment the soft block count (without expiration)
        //        MagicTRedisDatabase.MagicTRedisDb.StringSet(softBlockCountKey, softBlockCount);

        //        // Set the soft block duration with expiration
        //        MagicTRedisDatabase.MagicTRedisDb.StringSet(softBlockDurationKey, 1, TimeSpan.FromMinutes(RateLimiterConfig.SoftBlockDurationMinutes));
        //    }

        //    // Log the action taken
        //    Console.WriteLine($"Soft block added for {clientId}. Current soft block count is now {softBlockCount}.");
        //}


        /// <summary>
        /// Adds a hard block for a client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        private void AddHardBlock(string clientId)
        {
            const string redisKey = "HardBlockList";
            MagicTRedisDatabase.MagicTRedisDb.SetAdd(redisKey, clientId);
        }

        /// <summary>
        /// Removes a block for a client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        public void RemoveBlock(string clientId)
        {
            var softBlockCountKey = GetSoftBlockCountRedisKey(clientId);
            var softBlockDurationKey = GetSoftBlockDurationRedisKey(clientId);
            const string hardBlockKey = "HardBlockList";

            var script = @"
            redis.call('DEL', KEYS[1])
            redis.call('DEL', KEYS[2])
            redis.call('SREM', KEYS[3], ARGV[1])
            return 1";

            MagicTRedisDatabase.MagicTRedisDb.ScriptEvaluate(script, new RedisKey[] { softBlockCountKey, softBlockDurationKey, hardBlockKey }, new RedisValue[] { clientId });
        }
        //public void RemoveBlock(string clientId)
        //{
        //    var softBlockCountKey = GetSoftBlockCountRedisKey(clientId);
        //    var softBlockDurationKey = GetSoftBlockDurationRedisKey(clientId);

        //    MagicTRedisDatabase.MagicTRedisDb.KeyDelete(softBlockCountKey);
        //    MagicTRedisDatabase.MagicTRedisDb.KeyDelete(softBlockDurationKey);

        //    const string hardBlockKey = "HardBlockList";
        //    MagicTRedisDatabase.MagicTRedisDb.SetRemove(hardBlockKey, clientId);
        //}

        private int GetSoftBlockCount(string clientId)
        {
            var redisKey = GetSoftBlockCountRedisKey(clientId);
            var softBlockCount = MagicTRedisDatabase.MagicTRedisDb.StringGet(redisKey);

            if (!softBlockCount.HasValue)
            {
                Console.WriteLine($"Soft block count for {clientId} not found. Returning 0.");
                return 0;
            }

            Console.WriteLine($"Retrieved soft block count for {clientId}: {softBlockCount}");
            return (int)softBlockCount;
        }

        private int GetSoftBlockDuration(string clientId)
        {
            var key = GetSoftBlockDurationRedisKey(clientId);
            var duration = MagicTRedisDatabase.MagicTRedisDb.StringGet(key);

            if (!duration.HasValue)
            {
                Console.WriteLine($"Soft block duration for {clientId} not found. Returning 0.");
                return 0;
            }

            Console.WriteLine($"Retrieved soft block duration for {clientId}: {duration}");
            return (int)duration;
        }

        private string GetSoftBlockCountRedisKey(string clientId)
        {
            return $"SoftBlockCount:{clientId}";
        }

        private string GetSoftBlockDurationRedisKey(string clientId)
        {
            return $"SoftBlockDuration:{clientId}";
        }
    }
}

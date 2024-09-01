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
        private readonly MagicTRedisDatabase _magicTRedisDatabase;
        private readonly RateLimiterConfig _rateLimiterConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientBlockerService"/> class.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        /// <param name="configuration">The configuration settings.</param>
        public ClientBlockerService(IServiceProvider provider, IConfiguration configuration)
        {
            _magicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();
            _rateLimiterConfig = configuration.GetSection(nameof(RateLimiterConfig)).Get<RateLimiterConfig>();
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
            return _magicTRedisDatabase.MagicTRedisDb.SetContains(redisKey, clientId);
        }

        /// <summary>
        /// Adds a soft block for a client using Lua script for fewer database round trips and performance improvement.
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
            if (softBlockCount > _rateLimiterConfig.SoftBlockCount)
            {
                AddHardBlock(clientId);
            }
            else
            {
                // Increment the soft block count (without expiration)
                _magicTRedisDatabase.MagicTRedisDb.StringSet(softBlockCountKey, softBlockCount);

                // Set the soft block duration with expiration
                _magicTRedisDatabase.MagicTRedisDb.StringSet(softBlockDurationKey, 1, TimeSpan.FromMinutes(_rateLimiterConfig.SoftBlockDurationMinutes));
            }

            // Log the action taken
            Console.WriteLine($"Soft block added for {clientId}. Current soft block count is now {softBlockCount}.");
        }

        /// <summary>
        /// Adds a hard block for a client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        public void AddHardBlock(string clientId)
        {
            const string redisKey = "HardBlockList";
            _magicTRedisDatabase.MagicTRedisDb.SetAdd(redisKey, clientId);
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

            _magicTRedisDatabase.MagicTRedisDb.ScriptEvaluate(script, new RedisKey[] { softBlockCountKey, softBlockDurationKey, hardBlockKey }, new RedisValue[] { clientId });
        }

        /// <summary>
        /// Gets the soft block count for a client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns>The soft block count.</returns>
        private int GetSoftBlockCount(string clientId)
        {
            var redisKey = GetSoftBlockCountRedisKey(clientId);
            var softBlockCount = _magicTRedisDatabase.MagicTRedisDb.StringGet(redisKey);

            if (!softBlockCount.HasValue)
            {
                Console.WriteLine($"Soft block count for {clientId} not found. Returning 0.");
                return 0;
            }

            Console.WriteLine($"Retrieved soft block count for {clientId}: {softBlockCount}");
            return (int)softBlockCount;
        }

        /// <summary>
        /// Gets the soft block duration for a client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns>The soft block duration.</returns>
        private int GetSoftBlockDuration(string clientId)
        {
            var key = GetSoftBlockDurationRedisKey(clientId);
            var duration = _magicTRedisDatabase.MagicTRedisDb.StringGet(key);

            if (!duration.HasValue)
            {
                Console.WriteLine($"Soft block duration for {clientId} not found. Returning 0.");
                return 0;
            }

            Console.WriteLine($"Retrieved soft block duration for {clientId}: {duration}");
            return (int)duration;
        }

        /// <summary>
        /// Gets the Redis key for the soft block count of a client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns>The Redis key for the soft block count.</returns>
        private string GetSoftBlockCountRedisKey(string clientId)
        {
            return $"SoftBlockCount:{clientId}";
        }

        /// <summary>
        /// Gets the Redis key for the soft block duration of a client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns>The Redis key for the soft block duration.</returns>
        private string GetSoftBlockDurationRedisKey(string clientId)
        {
            return $"SoftBlockDuration:{clientId}";
        }
        
       
        /// <summary>
        /// Retrieves all clients that are either soft-blocked or hard-blocked.
        /// </summary>
        /// <returns>A list of client identifiers.</returns>
        public List<(string Id, string Ip, string BlockType)> ReadClients()
        {
            var clients = new List<(string Id, string Ip, string BlockType)>();

            // Retrieve all soft-blocked clients
            var softBlockKeysResult = _magicTRedisDatabase.MagicTRedisDb.Execute("KEYS", "SoftBlockCount:*");
            var softBlockKeys = (string[])softBlockKeysResult;

            if (softBlockKeys != null)
            {
                foreach (var key in softBlockKeys)
                {
                    var clientId = key?.Replace("SoftBlockCount:", string.Empty);
                    if (clientId != null)
                    {
                        clients.Add((clientId, clientId, "S")); // Assuming clientId is the IP
                    }
                }
            }

            // Retrieve all hard-blocked clients
            var hardBlockClients = _magicTRedisDatabase.MagicTRedisDb.SetMembers("HardBlockList");
            foreach (var client in hardBlockClients)
            {
                var clientId = (string)client;
                clients.Add((clientId, clientId, "P")); // Assuming clientId is the IP
            }

            return clients;
        }
    }
}
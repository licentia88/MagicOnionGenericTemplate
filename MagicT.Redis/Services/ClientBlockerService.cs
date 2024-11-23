using Benutomo;
using MagicT.Redis.Models;
using MagicT.Redis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace MagicT.Redis.Services
{
    /// <summary>
    /// Service for managing client blocking based on soft and hard block rules.
    /// </summary>
    [RegisterSingleton]
    [AutomaticDisposeImpl]
    public  partial class ClientBlockerService:IDisposable,IAsyncDisposable
    {
        [EnableAutomaticDispose]
        private readonly MagicTRedisDatabase _magicTRedisDatabase;
        
        [EnableAutomaticDispose]
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

        ~ClientBlockerService()
        {
            Dispose(false);
        }
        /// <summary>
        /// Checks if a client is soft-blocked.
        /// </summary>
        /// <param name="clientIp">The client identifier.</param>
        /// <returns><c>true</c> if the client is soft-blocked; otherwise, <c>false</c>.</returns>
        public bool IsSoftBlocked(string clientIp)
        {
            var softBlockDuration = GetSoftBlockDuration(clientIp);
            return softBlockDuration > 0;
        }

        /// <summary>
        /// Checks if a client is hard-blocked.
        /// </summary>
        /// <param name="clientIp">The client identifier.</param>
        /// <returns><c>true</c> if the client is hard-blocked; otherwise, <c>false</c>.</returns>
        public bool IsHardBlocked(string clientIp)
        {
            const string redisKey = "HardBlockList";
            return _magicTRedisDatabase.MagicTRedisDb.SetContains(redisKey, clientIp);
        }

        /// <summary>
        /// Adds a soft block for a client using Lua script for fewer database round trips and performance improvement.
        /// </summary>
        /// <param name="clientIp">The client identifier.</param>
        public void AddSoftBlock(string clientIp)
        {
            var softBlockCountKey = GetSoftBlockCountRedisKey(clientIp);
            var softBlockDurationKey = GetSoftBlockDurationRedisKey(clientIp);

            var script = @"
                        local softBlockCount = redis.call('INCR', KEYS[1])
                        if softBlockCount > tonumber(ARGV[1]) then
                            redis.call('SADD', KEYS[3], ARGV[2])
                        else
                            redis.call('SET', KEYS[2], ARGV[3], 'EX', ARGV[4])
                        end
                        return softBlockCount";

            var result = (int)_magicTRedisDatabase.MagicTRedisDb.ScriptEvaluate(script, new RedisKey[] { softBlockCountKey, softBlockDurationKey, "HardBlockList" }, new RedisValue[] { _rateLimiterConfig.SoftBlockCount, clientIp, _rateLimiterConfig.SoftBlockDurationMinutes, TimeSpan.FromMinutes(_rateLimiterConfig.SoftBlockDurationMinutes).TotalSeconds });

            Console.WriteLine($"Soft block added for {clientIp}. Current soft block count is now {result}.");
        }

        /// <summary>
        /// Adds a hard block for a client.
        /// </summary>
        /// <param name="clientIp">The client identifier.</param>
        public ClientData AddHardBlock(string clientIp)
        {
            const string redisKey = "HardBlockList";
            var softBlockCountKey = GetSoftBlockCountRedisKey(clientIp);
            var softBlockDurationKey = GetSoftBlockDurationRedisKey(clientIp);

            var script = @"
    redis.call('DEL', KEYS[2])
    redis.call('DEL', KEYS[3])
    redis.call('SADD', KEYS[1], ARGV[1])
    return redis.call('SISMEMBER', KEYS[1], ARGV[1])";

            var result = (int)_magicTRedisDatabase.MagicTRedisDb.ScriptEvaluate(script, new RedisKey[] { redisKey, softBlockCountKey, softBlockDurationKey }, new RedisValue[] { clientIp });

            var status = result == 1 ? "P" : "S";

            Console.WriteLine($"Hard block added for {clientIp}. Current status: {status}.");

            return new ClientData(clientIp, status, null);
        }
        /// <summary>
        /// Removes a block for a client.
        /// </summary>
        /// <param name="clientIp">The client identifier.</param>
        public void RemoveBlock(string clientIp)
        {
            var softBlockCountKey = GetSoftBlockCountRedisKey(clientIp);
            var softBlockDurationKey = GetSoftBlockDurationRedisKey(clientIp);
            const string hardBlockKey = "HardBlockList";

            var script = @"
            redis.call('DEL', KEYS[1])
            redis.call('DEL', KEYS[2])
            redis.call('SREM', KEYS[3], ARGV[1])
            return 1";

            _magicTRedisDatabase.MagicTRedisDb.ScriptEvaluate(script, new RedisKey[] { softBlockCountKey, softBlockDurationKey, hardBlockKey }, new RedisValue[] { clientIp });
        }

        /// <summary>
        /// Gets the soft block count for a client.
        /// </summary>
        /// <param name="clientIp">The client identifier.</param>
        /// <returns>The soft block count.</returns>
        private int GetSoftBlockCount(string clientIp)
        {
            var redisKey = GetSoftBlockCountRedisKey(clientIp);
            var softBlockCount = _magicTRedisDatabase.MagicTRedisDb.StringGet(redisKey);

            if (!softBlockCount.HasValue)
            {
                Console.WriteLine($"Soft block count for {clientIp} not found. Returning 0.");
                return 0;
            }

            Console.WriteLine($"Retrieved soft block count for {clientIp}: {softBlockCount}");
            return (int)softBlockCount;
        }

        /// <summary>
        /// Gets the soft block duration for a client.
        /// </summary>
        /// <param name="clientIp">The client identifier.</param>
        /// <returns>The soft block duration.</returns>
        private int GetSoftBlockDuration(string clientIp)
        {
            var key = GetSoftBlockDurationRedisKey(clientIp);
            var duration = _magicTRedisDatabase.MagicTRedisDb.StringGet(key);

            if (!duration.HasValue)
            {
                Console.WriteLine($"Soft block duration for {clientIp} not found. Returning 0.");
                return 0;
            }

            Console.WriteLine($"Retrieved soft block duration for {clientIp}: {duration}");
            return (int)duration;
        }

        /// <summary>
        /// Gets the Redis key for the soft block count of a client.
        /// </summary>
        /// <param name="clientIp">The client identifier.</param>
        /// <returns>The Redis key for the soft block count.</returns>
        private string GetSoftBlockCountRedisKey(string clientIp)
        {
            return $"SoftBlockCount_{clientIp}";
        }

        /// <summary>
        /// Gets the Redis key for the soft block duration of a client.
        /// </summary>
        /// <param name="clientIp">The client identifier.</param>
        /// <returns>The Redis key for the soft block duration.</returns>
        private string GetSoftBlockDurationRedisKey(string clientIp)
        {
            return $"SoftBlockDuration_{clientIp}";
        }

        /// <summary>
        /// Retrieves all clients that are either soft-blocked or hard-blocked.
        /// </summary>
        /// <returns>A list of client identifiers.</returns>
        public List<(string Ip, string BlockType, int? SoftBlockDuration)> ReadClients()
        {
            var clients = new List<(string Ip, string BlockType, int? SoftBlockDuration)>();

            // Retrieve all soft-blocked clients using the correct key pattern
            var softBlockKeysResult = _magicTRedisDatabase.MagicTRedisDb.Execute("KEYS", "SoftBlockDuration_*");
            var softBlockKeys = (string[])softBlockKeysResult;

            if (softBlockKeys != null && softBlockKeys.Length > 0)
            {
                clients.AddRange((from key in softBlockKeys select key?.Replace("SoftBlockDuration_", string.Empty) into clientIp where clientIp != null let softBlockDuration = GetSoftBlockDuration(clientIp) select (clientIp, "S", softBlockDuration)).Select(dummy => ((string Ip, string BlockType, int? SoftBlockDuration))dummy));
            }

            // Retrieve all hard-blocked clients
            var hardBlockClients = _magicTRedisDatabase.MagicTRedisDb.SetMembers("HardBlockList");
            clients.AddRange(hardBlockClients.Select(client => (string)client).Select(clientIp => (clientIp, "P", (int?)null)));

            return clients;
        }
    }
}
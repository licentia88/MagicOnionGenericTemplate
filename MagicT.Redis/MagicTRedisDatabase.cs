using Benutomo;
using MagicT.Redis.Extensions;
using MagicT.Redis.Options;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace MagicT.Redis;

/// <summary>
/// Provides a connection to a Redis database and exposes various methods for interacting with the Redis database.
/// </summary>
[AutomaticDisposeImpl]
[RegisterSingleton]
public  partial class MagicTRedisDatabase : IDisposable,IAsyncDisposable
{
    /// <summary>
    /// Configuration settings for the Redis connection.
    /// </summary>
   [EnableAutomaticDispose]
    private readonly MagicTRedisConfig _magicTRedisConfig;

    /// <summary>
    /// The connection multiplexer for connecting to Redis.
    /// </summary>
    [EnableAutomaticDispose]
    public readonly ConnectionMultiplexer Connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="MagicTRedisDatabase"/> class with the specified configuration.
    /// </summary>
    /// <param name="configuration">The configuration settings for connecting to the Redis database.</param>
    public MagicTRedisDatabase(IConfiguration configuration)
    {
        _magicTRedisConfig = configuration.GetSection("MagicTRedisConfig").Get<MagicTRedisConfig>();
        Connection = CreateConnectionMultiplexer();
    }

    ~MagicTRedisDatabase()
    {
        Dispose(false);
    }
    /// <summary>
    /// Gets the Redis database instance.
    /// </summary>
    public IDatabase MagicTRedisDb => Connection.GetDatabase();

    /// <summary>
    /// Creates and configures a new instance of <see cref="ConnectionMultiplexer"/> using the provided configuration.
    /// </summary>
    /// <returns>An instance of <see cref="ConnectionMultiplexer"/> connected to Redis.</returns>
    private ConnectionMultiplexer CreateConnectionMultiplexer()
    {
        return ConnectionMultiplexer.Connect(_magicTRedisConfig.ConnectionString);
    }

    /// <summary>
    /// Creates a new key-value pair in the Redis database.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key for the value.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="expiry">The expiration time for the key-value pair (optional).</param>
    public void Create<T>(string key, T value, TimeSpan? expiry = null)
    {
        var modelKey = $"{typeof(T).Name}:{key}";
        var serialized = value.SerializeToBytes();
        MagicTRedisDb.StringSet(modelKey, serialized, expiry);
    }

    /// <summary>
    /// Creates a new key-value pair in the Redis database asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key for the value.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="expiry">The expiration time for the key-value pair (optional).</param>
    public async Task CreateAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var modelKey = $"{typeof(T).Name}:{key}";
        var serialized = value.SerializeToBytes();
        await MagicTRedisDb.StringSetAsync(modelKey, serialized, expiry);
    }
    
    /// <summary>
    /// Adds or updates a key-value pair in the Redis database.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key for the value.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="expiry">The expiration time for the key-value pair (optional).</param>
    public void AddOrUpdate<T>(string key, T value, TimeSpan? expiry = null)
    {
        var modelKey = $"{typeof(T).Name}:{key}";
        var serialized = value.SerializeToBytes();
        MagicTRedisDb.StringSet(modelKey, serialized, expiry);
    }

    /// <summary>
    /// Adds or updates a key-value pair in the Redis database asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key for the value.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="expiry">The expiration time for the key-value pair (optional).</param>
    public async Task AddOrUpdateAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var modelKey = $"{typeof(T).Name}:{key}";
        var serialized = value.SerializeToBytes();
        await MagicTRedisDb.StringSetAsync(modelKey, serialized, expiry);
    }
    
    /// <summary>
    /// Retrieves the value associated with the specified key from the Redis database.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key to retrieve.</param>
    /// <returns>The value associated with the key, or default if the key is not found.</returns>
    public T ReadAs<T>(string key)
    {
        var modelKey = $"{typeof(T).Name}:{key}";
        byte[] value = MagicTRedisDb.StringGet(modelKey);
        return value is null ? default : value.DeserializeFromBytes<T>();
    }
    
    /// <summary>
    /// Retrieves the value associated with the specified key from the Redis database asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key to retrieve.</param>
    /// <returns>The value associated with the key, or default if the key is not found.</returns>
    public async Task<T> ReadAsAsync<T>(string key)
    {
        var modelKey = $"{typeof(T).Name}:{key}";
        byte[] value = await MagicTRedisDb.StringGetAsync(modelKey);
        return value is null ? default : value.DeserializeFromBytes<T>();
    }

    /// <summary>
    /// Updates an existing key-value pair in the Redis database.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key for the value.</param>
    /// <param name="value">The new value to store.</param>
    /// <param name="expiry">The expiration time for the key-value pair (optional).</param>
    public void Update<T>(string key, T value, TimeSpan? expiry = null)
    {
        if (!MagicTRedisDb.KeyExists(key)) return;

        var modelKey = $"{typeof(T).Name}:{key}";
        var serialized = value.SerializeToBytes();
        MagicTRedisDb.StringSet(modelKey, serialized, expiry);
    }

    
    /// <summary>
    /// Updates an existing key-value pair in the Redis database asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key for the value.</param>
    /// <param name="value">The new value to store.</param>
    /// <param name="expiry">The expiration time for the key-value pair (optional).</param>
    public async Task UpdateAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        if (!await MagicTRedisDb.KeyExistsAsync(key)) return;

        var modelKey = $"{typeof(T).Name}:{key}";
        var serialized = value.SerializeToBytes();
        await MagicTRedisDb.StringSetAsync(modelKey, serialized, expiry);
    }
    
    
    /// <summary>
    /// Deletes the key-value pair associated with the specified key from the Redis database.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key to delete.</param>
    public void Delete<T>(string key)
    {
        var modelKey = $"{typeof(T).Name}:{key}";
        MagicTRedisDb.KeyDelete(modelKey);
    }

    /// <summary>
    /// Deletes the key-value pair associated with the specified key from the Redis database asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key to delete.</param>
    public async Task DeleteAsync<T>(string key)
    {
        var modelKey = $"{typeof(T).Name}:{key}";
        await MagicTRedisDb.KeyDeleteAsync(modelKey);
    }

    
    /// <summary>
    /// Adds an element to the end of a list stored at the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key of the list.</param>
    /// <param name="value">The value to add to the list.</param>
    public void Push<T>(string key, T value)
    {
        var modelKey = $"{typeof(T).Name}:{key}";
        var serialized = value.SerializeToBytes();
        MagicTRedisDb.ListRightPush(modelKey, serialized);
    }

    
    /// <summary>
    /// Adds an element to the end of a list stored at the specified key asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key of the list.</param>
    /// <param name="value">The value to add to the list.</param>
    public async Task PushAsync<T>(string key, T value)
    {
        var modelKey = $"{typeof(T).Name}:{key}";
        var serialized = value.SerializeToBytes();
        await MagicTRedisDb.ListRightPushAsync(modelKey, serialized);
    }
    
    /// <summary>
    /// Retrieves all elements from a list stored at the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="key">The key of the list.</param>
    /// <returns>An array of elements from the list.</returns>
    public T[] PullAs<T>(string key)
    {
        var modelKey = $"{typeof(T).Name}:{key}";
        return MagicTRedisDb.ListRange(modelKey).Select(x => ((byte[])x).DeserializeFromBytes<T>()).ToArray();
    }
    
    
    /// <summary>
    /// Retrieves all elements from a list stored at the specified key asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="key">The key of the list.</returns>
    /// <returns>An array of elements from the list.</returns>
    public async Task<T[]> PullAsAsync<T>(string key)
    {
        var modelKey = $"{typeof(T).Name}:{key}";
        var values = await MagicTRedisDb.ListRangeAsync(modelKey);
        return values.Select(x => ((byte[])x).DeserializeFromBytes<T>()).ToArray();
    }
    
    /// <summary>
    /// Attempts to acquire a distributed lock on a given key with an expiration time.
    /// </summary>
    /// <param name="key">The key representing the lock.</param>
    /// <param name="expiry">The expiration time for the lock. Default is 30 seconds.</param>
    /// <returns>True if the lock was acquired, false otherwise.</returns>
    public bool Lock<TModel>(string key, TimeSpan? expiry = null)
    {
        var lockKey = $"lock_{typeof(TModel).Name}:{key}";
        expiry ??= TimeSpan.FromSeconds(30);

        // Try to set the lock with NX (only if not exists) and set expiration time
        return MagicTRedisDb.StringSet(lockKey, "locked", expiry, When.NotExists);
    }

    
    /// <summary>
    /// Attempts to acquire a distributed lock on a given key with an expiration time.
    /// </summary>
    /// <param name="key">The key representing the lock.</param>
    /// <param name="expiry">The expiration time for the lock. Default is 30 seconds.</param>
    /// <returns>True if the lock was acquired, false otherwise.</returns>
    public async Task<bool> LockAsync<TModel>(string key, TimeSpan? expiry = null)
    {
        var lockKey = $"lock_{typeof(TModel).Name}:{key}";
        expiry ??= TimeSpan.FromSeconds(30);

        // Try to set the lock with NX (only if not exists) and set expiration time
        return await MagicTRedisDb.StringSetAsync(lockKey, "locked", expiry, When.NotExists);
    }

    /// <summary>
    /// Releases the distributed lock on a given key.
    /// </summary>
    /// <param name="key">The key representing the lock.</param>
    /// <returns>True if the lock was successfully released, false otherwise.</returns>
    public bool Unlock<TModel>(string key)
    {
        var lockKey = $"lock_{typeof(TModel).Name}:{key}";
        return MagicTRedisDb.KeyDelete(lockKey);
    }
    
    /// <summary>
    /// Releases the distributed lock on a given key.
    /// </summary>
    /// <param name="key">The key representing the lock.</param>
    /// <returns>True if the lock was successfully released, false otherwise.</returns>
    public async Task<bool> UnlockAsync<TModel>(string key)
    {
        var lockKey = $"lock_{typeof(TModel).Name}:{key}";
        return await MagicTRedisDb.KeyDeleteAsync(lockKey);
    }
    
    
}
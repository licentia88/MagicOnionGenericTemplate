﻿using System.Text.Json;
using Benutomo;
using StackExchange.Redis;

namespace MagicT.Redis;

/// <summary>
///     Provides a connection to a Redis database and exposes the Redis database instance.
/// </summary>
[AutomaticDisposeImpl]
public sealed partial class MagicTRedisDatabase:IDisposable,IAsyncDisposable
{
    [EnableAutomaticDispose]
    RedisConnectionManager RedisConnectionManager { get; set; }

    /// <summary>
    ///     Gets the Redis database instance.
    /// </summary>
    public IDatabase MagicTRedisDb => Connection.GetDatabase();

    /// <summary>
    ///     Gets the Redis connection instance.
    /// </summary>
    [EnableAutomaticDispose]
    private ConnectionMultiplexer Connection => RedisConnectionManager.ConnectionMultiplexer;

    /// <summary>
    ///     Initializes a new instance of the MagicTRedisDatabase class with the specified configuration.
    /// </summary>
    /// <param name="redisConnectionManager"></param>
    public MagicTRedisDatabase(RedisConnectionManager redisConnectionManager)
    {
        RedisConnectionManager = redisConnectionManager;
    }


    public void Create<T>(string key, T value, TimeSpan? expiry = null)
    {
        var modelKey = $"{typeof(T).Name}_{key}";  

        var serialized = JsonSerializer.Serialize(value);

        MagicTRedisDb.StringSet(modelKey, serialized, expiry);
    }

    public void AddOrUpdate<T>(string key, T value, TimeSpan? expiry = null)
    {
        var modelKey = $"{typeof(T).Name}_{key}";

        var serialized = JsonSerializer.Serialize(value);

        MagicTRedisDb.StringSet(modelKey, serialized, expiry);
    }

    /// <summary>
    ///     Retrieves the value associated with the specified key from the Redis database.
    /// </summary>
    /// <param name="key">The key to retrieve.</param>
    /// <returns>The value associated with the key, or null if the key is not found.</returns>
    public T ReadAs<T>(string key)  
    {
        var modelKey = $"{typeof(T).Name}_{key}";
        var value = MagicTRedisDb.StringGet(modelKey);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value) : default;
    }

    public void Update<T>(string key, T newValue, TimeSpan? expiry = null)
    {
        if (!MagicTRedisDb.KeyExists(key)) return;

        var modelKey = $"{typeof(T).Name}_{key}";
        var serialized = JsonSerializer.Serialize(newValue);

        MagicTRedisDb.StringSet(modelKey, serialized, expiry);
    }

    /// <summary>
    ///     Deletes the key-value pair associated with the specified key from the Redis database.
    /// </summary>
    /// <param name="key">The key to delete.</param>
    public void Delete<T>(string key)
    {
        var modelKey = $"{typeof(T).Name}_{key}";
        MagicTRedisDb.KeyDelete(modelKey);
    }


    /// <summary>
    /// Adds an element to the end of a list stored at the specified key.
    /// </summary>
    /// <param name="key">The key of the list.</param>
    /// <param name="value">The value to add to the list.</param>
    public void Push<T>(string key, T value)
    {
        var modelKey = $"{typeof(T).Name}_{key}";
        var serialized = JsonSerializer.Serialize(value);
        MagicTRedisDb.ListRightPush(modelKey, serialized);
    }

    
    /// <summary>
    /// Retrieves all elements from a list stored at the specified key.
    /// </summary>
    /// <param name="key">The key of the list.</param>
    /// <returns>An array of elements from the list.</returns>
    
    public T[] PullAs<T>(string key)
    {
        var modelKey = $"{typeof(T).Name}_{key}";

        return MagicTRedisDb.ListRange(modelKey).Select(x => JsonSerializer.Deserialize<T>(x)).ToArray();
    }
}
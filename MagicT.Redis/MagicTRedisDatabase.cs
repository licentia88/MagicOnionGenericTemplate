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
public sealed partial class MagicTRedisDatabase : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Configuration settings for the Redis connection.
    /// </summary>
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
}
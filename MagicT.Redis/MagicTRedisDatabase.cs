using System.Text.Json;
using MagicT.Redis.Options;
using StackExchange.Redis;

namespace MagicT.Redis;

/// <summary>
///     Provides a connection to a Redis database and exposes the Redis database instance.
/// </summary>
public sealed class MagicTRedisDatabase
{
    private readonly Lazy<ConnectionMultiplexer> lazyConnection;

    private readonly MagicTRedisConfig MagicTRedisConfig;

    /// <summary>
    ///     Initializes a new instance of the MagicTRedisDatabase class with the specified configuration.
    /// </summary>
    /// <param name="configuration">The IConfiguration instance used to read the Redis connection string.</param>
    public MagicTRedisDatabase(MagicTRedisConfig configuration)
    {
        MagicTRedisConfig = configuration;

        lazyConnection = new Lazy<ConnectionMultiplexer>(CreateConnectionMultiplexer);
    }

    /// <summary>
    ///     Gets the Redis database instance.
    /// </summary>
    public IDatabase MagicTRedisDb => Connection.GetDatabase();

    /// <summary>
    ///     Gets the Redis connection instance.
    /// </summary>
    private ConnectionMultiplexer Connection => lazyConnection.Value;

    private ConnectionMultiplexer CreateConnectionMultiplexer()
    {
#if Docker
        return ConnectionMultiplexer.Connect(MagicTRedisConfig.ConnectionStringDocker);
#else
        return ConnectionMultiplexer.Connect(MagicTRedisConfig.ConnectionString);
#endif
        // Read the Redis connection string from the configuration section "MagicTRedisDatabase"
    }

    /// <summary>
    ///     Inserts a key-value pair into the Redis database.
    /// </summary>
    /// <param name="key">The key to insert.</param>
    /// <param name="value">The value to insert.</param>
    public void Create<T>(string key, T value)
    {
         Create(key, value, null);
    }

    public void Create<T>(string key, T value, TimeSpan? expiry)
    {
        var modelKey = typeof(T).Name + key;

        var serialized = JsonSerializer.Serialize(value);

        MagicTRedisDb.StringSet(modelKey, serialized, expiry);
    }

    /// <summary>
    /// Adds or updates a key-value pair in the Redis database.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key to add or update.</param>
    /// <param name="value">The value to add or update.</param>
    /// <param name="expiry">The optional expiration time for the key.</param>
    public void AddOrUpdate<T>(string key, T value)
    {
        AddOrUpdate(key, value,null);
    }

    public void AddOrUpdate<T>(string key, T value, TimeSpan? expiry)
    {
        var modelKey = typeof(T).Name + key;

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
        var modelKey = typeof(T).Name + key;
        var value = MagicTRedisDb.StringGet(modelKey);

        return JsonSerializer.Deserialize<T>(value);
        //return (T)Convert.ChangeType(value, typeof(T));
    }

    /// <summary>
    ///     Updates the value associated with the specified key in the Redis database.
    /// </summary>
    /// <param name="key">The key to update.</param>
    /// <param name="newValue">The new value to set.</param>
    public void Update<T>(string key, T newValue)
    {
        Update(key, newValue, null);
    }

    public void Update<T>(string key, T newValue, TimeSpan? expiry)
    {
        if (!MagicTRedisDb.KeyExists(key)) return;

        var modelKey = typeof(T).Name + key;
        var serialized = JsonSerializer.Serialize(newValue);

        MagicTRedisDb.StringSet(modelKey, serialized, expiry);
    }

    /// <summary>
    ///     Deletes the key-value pair associated with the specified key from the Redis database.
    /// </summary>
    /// <param name="key">The key to delete.</param>
    public void Delete<T>(string key)
    {
        var modelKey = typeof(T).Name + key;
        MagicTRedisDb.KeyDelete(modelKey);
    }


    /// <summary>
    /// Adds an element to the end of a list stored at the specified key.
    /// </summary>
    /// <param name="key">The key of the list.</param>
    /// <param name="value">The value to add to the list.</param>
    public void Push<T>(string key, T value)
    {
        var modelKey = typeof(T).Name + key;
        var serialized = JsonSerializer.Serialize(value);
        MagicTRedisDb.ListRightPush(modelKey, serialized);
    }

    //public void PushBytes<T>(string key, byte[] value)
    //{
    //    var modelKey = typeof(T).Name + key;
    //    MagicTRedisDb.ListRightPush(modelKey, value);
    //}
    /// <summary>
    /// Retrieves all elements from a list stored at the specified key.
    /// </summary>
    /// <param name="key">The key of the list.</param>
    /// <returns>An array of elements from the list.</returns>
    
    public T[] PullAs<T>(string key)
    {
        var modelKey = typeof(T).Name + key;
 
        return MagicTRedisDb.ListRange(modelKey).Select(x => JsonSerializer.Deserialize<T>(x)).ToArray();
    }
}
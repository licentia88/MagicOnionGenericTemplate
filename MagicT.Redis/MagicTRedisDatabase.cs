using MagicT.Redis.Options;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace MagicT.Redis;

/// <summary>
/// Provides a connection to a Redis database and exposes the Redis database instance.
/// </summary>
public class MagicTRedisDatabase
{
    private readonly Lazy<ConnectionMultiplexer> lazyConnection;

    /// <summary>
    /// Gets the Redis database instance.
    /// </summary>
    public IDatabase MagicTRedisDb => Connection.GetDatabase();

    /// <summary>
    /// Gets the Redis connection instance.
    /// </summary>
    private ConnectionMultiplexer Connection => lazyConnection.Value;

    private MagicTRedisConfig MagicTRedisConfig;
    /// <summary>
    /// Initializes a new instance of the MagicTRedisDatabase class with the specified configuration.
    /// </summary>
    /// <param name="configuration">The IConfiguration instance used to read the Redis connection string.</param>
    public MagicTRedisDatabase(MagicTRedisConfig config)
    {
        MagicTRedisConfig = config;

        lazyConnection = new Lazy<ConnectionMultiplexer>(CreateConnectionMultiplexer);
    }

    private ConnectionMultiplexer CreateConnectionMultiplexer()
    {
        // Read the Redis connection string from the configuration section "MagicTRedisDatabase"
        return ConnectionMultiplexer.Connect(MagicTRedisConfig.ConnectionString);
    }



}



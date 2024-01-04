namespace MagicT.Redis.Options;

/// <summary>
/// Configuration options for MagicT Redis integration.
/// </summary>
public sealed class MagicTRedisConfig
{
    /// <summary>
    /// The Redis connection string used to connect to the Redis server.
    /// </summary>
    public string ConnectionString { get; set; }
 
}

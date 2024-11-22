using Benutomo;

namespace MagicT.Redis.Options;

/// <summary>
/// Configuration options for MagicT Redis integration.
/// </summary>

[AutomaticDisposeImpl]
public partial class  MagicTRedisConfig:IDisposable
{
    /// <summary>
    /// The Redis connection string used to connect to the Redis server.
    /// </summary>
    public string ConnectionString { get; set; }
 
    ~MagicTRedisConfig()
    {
        Dispose();
    }
}

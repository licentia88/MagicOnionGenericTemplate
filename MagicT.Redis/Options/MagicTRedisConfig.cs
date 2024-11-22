using Benutomo;

namespace MagicT.Redis.Options;

/// <summary>
/// Configuration options for MagicT Redis integration.
/// </summary>

[AutomaticDisposeImpl]
public sealed partial class  MagicTRedisConfig:IDisposable, IAsyncDisposable
{
    /// <summary>
    /// The Redis connection string used to connect to the Redis server.
    /// </summary>
    public string ConnectionString { get; set; }
 
    ~MagicTRedisConfig()
    {
        Dispose();
        GC.WaitForPendingFinalizers();
    }
}

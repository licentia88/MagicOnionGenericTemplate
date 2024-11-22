using Benutomo;

namespace MagicT.Redis.Options;


/// <summary>
/// Configuration options for rate limiting settings.
/// </summary>
[AutomaticDisposeImpl]
public sealed partial class RateLimiterConfig:IDisposable, IAsyncDisposable
{
    /// <summary>
    /// The maximum number of requests allowed within the specified time period.
    /// </summary>
    public int RateLimit { get; set; }

    /// <summary>
    /// The time interval (in seconds) during which the rate limit applies.
    /// </summary>
    public int PerSecond { get; set; }


    /// <summary>
    /// The number of requests that trigger a soft block.
    /// </summary>
    public int SoftBlockCount { get; set; }

    /// <summary>
    /// The duration in minutes for which soft blocking occurs.
    /// </summary>
    public int SoftBlockDurationMinutes { get; set; }

    // No constructor or methods are present in this class.
    
    ~RateLimiterConfig()
    {
        Dispose();
        GC.WaitForPendingFinalizers();
    }
}

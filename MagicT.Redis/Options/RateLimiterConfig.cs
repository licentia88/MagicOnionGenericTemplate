namespace MagicT.Redis.Options;


/// <summary>
/// Configuration options for rate limiting settings.
/// </summary>
public class RateLimiterConfig
{
    /// <summary>
    /// Gets or sets the maximum rate limit.
    /// </summary>
    public int RateLimit { get; set; }
    // The maximum number of requests allowed within the specified time period.

    /// <summary>
    /// Gets or sets the time interval (in seconds) for the rate limit.
    /// </summary>
    public int PerSecond { get; set; }
    // The time interval (in seconds) during which the rate limit applies.

    /// <summary>
    /// Gets or sets the soft block count.
    /// </summary>
    public int SoftBlockCount { get; set; }
    // The number of requests that trigger a soft block.

    /// <summary>
    /// Gets or sets the soft block duration in minutes.
    /// </summary>
    public int SoftBlockDurationMinutes { get; set; }
    // The duration in minutes for which soft blocking occurs.

    // No constructor or methods are present in this class.
}

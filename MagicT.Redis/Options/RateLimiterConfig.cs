namespace MagicT.Redis.Options;

public class RateLimiterConfig
{
    public int RateLimit { get; set; }
    public int PerSecond { get; set; }
    public int SoftBlockCount { get; set; }
    public int SoftBlockDurationMinutes { get; set; }
}



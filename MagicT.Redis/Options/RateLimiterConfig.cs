using System;
namespace MagicT.Redis.Options;

public class RateLimiterConfig
{
    public int RateLimit { get; set; }
    public int TimeWindowMinutes { get; set; }
    public int SoftBlockCount { get; set; }
    public int SoftBlockDurationHours { get; set; }
}



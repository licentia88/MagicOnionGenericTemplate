namespace MagicT.Redis.Options;

/// <summary>
/// Configuration options for token service settings.
/// </summary>
public sealed class TokenServiceConfig
{
    /// <summary>
    ///  The duration in minutes for which tokens are considered valid.
    /// </summary>
    public int TokenExpirationMinutes { get; set; }

}

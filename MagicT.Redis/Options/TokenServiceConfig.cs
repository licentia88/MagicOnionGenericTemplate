namespace MagicT.Redis.Options;

/// <summary>
/// Configuration options for token service settings.
/// </summary>
public sealed class TokenServiceConfig
{
    /// <summary>
    /// Gets or sets the token expiration time in minutes.
    /// </summary>
    public int TokenExpirationMinutes { get; set; }
    // The duration in minutes for which tokens are considered valid.

    // No constructor or methods are present in this class.
}

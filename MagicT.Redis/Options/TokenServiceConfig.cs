using Benutomo;

namespace MagicT.Redis.Options;

/// <summary>
/// Configuration options for token service settings.
/// </summary>
[AutomaticDisposeImpl]
public partial class TokenServiceConfig:IDisposable
{
    /// <summary>
    ///  The duration in minutes for which tokens are considered valid.
    /// </summary>
    public int TokenExpirationMinutes { get; set; }

}

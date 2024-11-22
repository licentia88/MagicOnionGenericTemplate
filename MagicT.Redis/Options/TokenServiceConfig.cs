using Benutomo;

namespace MagicT.Redis.Options;

/// <summary>
/// Configuration options for token service settings.
/// </summary>
[AutomaticDisposeImpl]
public sealed partial class TokenServiceConfig:IDisposable, IAsyncDisposable
{
    /// <summary>
    ///  The duration in minutes for which tokens are considered valid.
    /// </summary>
    public int TokenExpirationMinutes { get; set; }

}

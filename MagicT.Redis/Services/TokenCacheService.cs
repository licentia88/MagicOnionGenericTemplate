using MagicT.Redis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Redis.Services;

/// <summary>
/// Service for caching and retrieving user tokens using Redis.
/// </summary>
public sealed class TokenCacheService
{
    private readonly MagicTRedisDatabase MagicTRedisDatabase;
    private readonly TokenServiceConfig TokenServiceConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenCacheService"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public TokenCacheService(IServiceProvider provider, IConfiguration configuration)
    {
        MagicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();
        TokenServiceConfig = configuration.GetSection(nameof(TokenServiceConfig)).Get<TokenServiceConfig>();

    }

    /// <summary>
    /// Caches a token for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="binaryToken">The binary token to cache.</param>
    public void CacheToken(int userId, byte[] binaryToken)
    {
        var tokenKey = $"Token:{userId}";
        MagicTRedisDatabase.MagicTRedisDb.StringSet(tokenKey, binaryToken,
            TimeSpan.FromMinutes(TokenServiceConfig.TokenExpirationMinutes));
        // Caches the provided binary token with a specified expiration time.
    }

    /// <summary>
    /// Retrieves a cached token for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The cached binary token, or null if not found.</returns>
    public byte[] GetCachedToken(int userId)
    {
        var tokenKey = $"Token:{userId}";
        return MagicTRedisDatabase.MagicTRedisDb.StringGet(tokenKey);
        // Retrieves the cached binary token associated with the specified user.
    }
}
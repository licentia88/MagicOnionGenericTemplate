using Benutomo;
using MagicT.Redis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Redis.Services;

/// <summary>
/// Service for caching and retrieving user tokens using Redis.
/// </summary>
[AutomaticDisposeImpl]
public  partial class TokenCacheService:IDisposable
{
    [EnableAutomaticDispose]
    private readonly MagicTRedisDatabase _magicTRedisDatabase;
    
    [EnableAutomaticDispose]
    private readonly TokenServiceConfig _tokenServiceConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenCacheService"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    /// <param name="configuration">The configuration settings.</param>
    public TokenCacheService(IServiceProvider provider, IConfiguration configuration)
    {
        _magicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();
        _tokenServiceConfig = configuration.GetSection(nameof(TokenServiceConfig)).Get<TokenServiceConfig>();
    }

    ~TokenCacheService()
    {
        Dispose();
    }
    /// <summary>
    /// Caches a token for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="binaryToken">The binary token to cache.</param>
    public void CacheToken(int userId, byte[] binaryToken)
    {
        var tokenKey = GetTokenRedisKey(userId);
        _magicTRedisDatabase.MagicTRedisDb.StringSet(tokenKey, binaryToken, TimeSpan.FromMinutes(_tokenServiceConfig.TokenExpirationMinutes));
    }

    /// <summary>
    /// Retrieves a cached token for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The cached binary token, or null if not found.</returns>
    public byte[] GetCachedToken(int userId)
    {
        var tokenKey = GetTokenRedisKey(userId);
        return _magicTRedisDatabase.MagicTRedisDb.StringGet(tokenKey);
    }

    /// <summary>
    /// Gets the Redis key for the token of a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The Redis key for the token.</returns>
    private string GetTokenRedisKey(int userId)
    {
        return $"Token:{userId}";
    }
}
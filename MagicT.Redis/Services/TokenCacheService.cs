using MagicT.Redis.Options;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Redis.Services;

public class TokenCacheService
{
    private readonly MagicTRedisDatabase MagicTRedisDatabase;

    private readonly TokenServiceConfig TokenServiceConfig;


    public TokenCacheService(IServiceProvider provider)
    {
        MagicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();
        TokenServiceConfig = provider.GetService<TokenServiceConfig>();
    }

    public void CacheToken(int userId, byte[] binaryToken)
    {
        var tokenKey = $"Token:{userId}";

        MagicTRedisDatabase.MagicTRedisDb.StringSet(tokenKey, binaryToken,
            TimeSpan.FromMinutes(TokenServiceConfig.TokenExpirationMinutes));
    }

    public byte[] GetCachedToken(int userId)
    {
        var tokenKey = $"Token:{userId}";
        return MagicTRedisDatabase.MagicTRedisDb.StringGet(tokenKey);
    }
}
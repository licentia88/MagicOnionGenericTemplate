using MagicT.Redis.Options;
using MagicT.Redis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Redis.Extensions;

public static class DependencyExtensions
{
    public static void RegisterRedisDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddMessagePipe();
        //services.AddMessagePipeRedis(ConnectionMultiplexer.Connect("localhost:6379"));
        // IDistributedCache Configuration
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "localhost";
            options.InstanceName = "MagicTRedisInstance";
        });

        // Inject MagicTRedisDatabase as a singleton
        services.AddSingleton<MagicTRedisDatabase>();

        // Inject RateLimiter as a singleton service
        services.AddSingleton<RateLimiterService>();

        // Inject ClientBlocker as a singleton service
        services.AddSingleton<ClientBlockerService>();

        services.AddSingleton<TokenCacheService>();

        services.AddSingleton(_ => configuration.GetSection("RateLimiterConfig").Get<RateLimiterConfig>());

        services.AddSingleton(_ => configuration.GetSection("MagicTRedisConfig").Get<MagicTRedisConfig>());

        services.AddSingleton(_ => configuration.GetSection("TokenServiceConfig").Get<TokenServiceConfig>());
    }
}
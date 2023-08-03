using MagicT.Redis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Redis.Extensions;

public static class DependencyExtensions
{
    public static void RegisterRedisDatabase(this IServiceCollection services, IConfiguration configuration)
    {
       
        // Inject MagicTRedisDatabase as a singleton
        services.AddSingleton<MagicTRedisDatabase>();

        // Inject RateLimiter as a singleton service
        services.AddSingleton<RateLimiter>();

        // Inject ClientBlocker as a singleton service
        services.AddSingleton<ClientBlocker>();

        
        services.AddSingleton(_ => configuration.GetSection("RateLimiterConfig").Get<RateLimiterConfig>());

        services.AddSingleton(_ => configuration.GetSection("MagicTRedisConfig").Get<MagicTRedisConfig>());


    }

}
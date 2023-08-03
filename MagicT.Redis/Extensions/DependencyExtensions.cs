using System.Reflection;
using MagicT.Redis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Redis.Extensions;

public static class DependencyExtensions
{
    public static void RegisterRedisDatabase(this IServiceCollection Services, IConfiguration configuration)
    {
       
        // Inject MagicTRedisDatabase as a singleton
        Services.AddSingleton<MagicTRedisDatabase>();

        // Inject RateLimiter as a singleton service
        Services.AddSingleton<RateLimiter>();

        // Inject ClientBlocker as a singleton service
        Services.AddSingleton<ClientBlocker>();

        
        Services.AddSingleton(x => configuration.GetSection("RateLimiterConfig").Get<RateLimiterConfig>());

        Services.AddSingleton(x => configuration.GetSection("MagicTRedisConfig").Get<MagicTRedisConfig>());


    }

}
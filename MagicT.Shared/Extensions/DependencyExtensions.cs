using MagicT.Redis;
using MagicT.Shared.Formatters;
using MagicT.Shared.Managers;
using MagicT.Shared.Models;
using MagicT.Shared.Serializers;
using MessagePipe;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Shared.Extensions;

/// <summary>
/// Provides extension methods for registering shared services.
/// </summary>
public static class DependencyExtensions
{
    /// <summary>
    /// Registers shared services and configurations.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="configuration">The application configuration.</param>
    public static void RegisterShared(this IServiceCollection services, IConfiguration configuration)
    {
        MemoryPackFormatterProvider.Register(new UnsafeObjectFormatter());
        // MemoryPackFormatterProvider.Register(new MultiArrayFormatter<Vector3MultiArray>());
        
        services.AddMessagePipe(options =>
        {
            options.InstanceLifetime = InstanceLifetime.Singleton;
#if DEBUG
            // EnableCaptureStackTrace slows performance, so recommended to use only in DEBUG and in profiling, disable it.
            options.EnableCaptureStackTrace = true;
#endif
        });

        var redisDatabase = new MagicTRedisDatabase(configuration);

        services.AddMessagePipeRedis(redisDatabase.Connection, x =>
        {
            x.RedisSerializer = new RedisMemoryPackSerializer();
        });

        services.AddSingleton(typeof(LogManager<>));

    }
}
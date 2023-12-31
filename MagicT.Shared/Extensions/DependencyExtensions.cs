using MagicOnion;
using MagicT.Redis;
using MagicT.Shared.Formatters;
using MagicT.Shared.Serializers;
using MemoryPack;
using MessagePipe;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Shared.Extensions;

public static class DependencyExtensions
{
    public static void RegisterShared(this IServiceCollection services, IConfiguration configuration)
    {
        MemoryPackFormatterProvider.Register(new UnsafeObjectFormatter());

        services.AddMessagePipe(options =>
        {
            options.InstanceLifetime = InstanceLifetime.Scoped;
#if DEBUG
            // EnableCaptureStackTrace slows performance, so recommended to use only in DEBUG and in profiling, disable it.
            options.EnableCaptureStackTrace = true;
#endif
        });

        var connectionManager = new RedisConnectionManager(configuration);

        services.AddMessagePipeRedis(connectionManager.ConnectionMultiplexer, x =>
        {
            x.RedisSerializer = new RedisMemoryPackSerializer();
        });
    }
}

using MagicT.Redis;
using MagicT.Shared.Formatters;
using MagicT.Shared.Managers;
using MagicT.Shared.Models;
using MagicT.Shared.Serializers;
using MessagePipe;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Shared.Extensions;

public static class DependencyExtensions
{
    public static void RegisterShared(this IServiceCollection services, IConfiguration configuration)
    {
        MemoryPackFormatterProvider.Register(new UnsafeObjectFormatter());

 
        //MemoryPackFormatterProvider.Register((FuncFormatter<>)

        services.AddMessagePipe(options =>
        {
            options.InstanceLifetime = InstanceLifetime.Singleton;
//-:cnd
#if DEBUG
            // EnableCaptureStackTrace slows performance, so recommended to use only in DEBUG and in profiling, disable it.
            options.EnableCaptureStackTrace = true;
#endif
//+:cnd
        });

        var redisDatabase = new MagicTRedisDatabase(configuration);

        services.AddMessagePipeRedis(redisDatabase.Connection, x =>
        {
            x.RedisSerializer = new RedisMemoryPackSerializer();
        });

        services.AddSingleton(typeof(LogManager<>));
    }
}

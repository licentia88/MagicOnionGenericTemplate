using MagicT.Redis;
using MagicT.Shared.Serializers;
using MessagePipe;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Shared.Extensions;


[Obsolete("do not use", true)]
public static class MessagePipeExtensions
{

    public static void RegisterPipes(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddMessagePipe();

        services.AddMessagePipe(options =>
        {
            options.InstanceLifetime = InstanceLifetime.Scoped;
//-:cnd
#if DEBUG
            // EnableCaptureStackTrace slows performance, so recommended to use only in DEBUG and in profiling, disable it.
            options.EnableCaptureStackTrace = true;
#endif
//+:cnd
        });

        var connectionManager = new RedisConnectionManager(configuration);
        //var host = Dns.GetHostEntry("magictserver");
        //var hostIp = host.AddressList.Last().ToString();
        //services.AddMessagePipeTcpInterprocess(hostIp, 5029, x => x.InstanceLifetime = InstanceLifetime.Singleton);

        services.AddMessagePipeRedis(connectionManager.ConnectionMultiplexer, x =>
        {
            x.RedisSerializer = new RedisMemoryPackSerializer();
        });
        //services.AddMessagePipeTcpInterprocess("127.0.0.1", 5029, x => x.InstanceLifetime = InstanceLifetime.Singleton);
    }
}
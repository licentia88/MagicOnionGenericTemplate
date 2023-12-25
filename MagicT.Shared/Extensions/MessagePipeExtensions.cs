using MessagePipe;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Shared.Extensions;

public static class MessagePipeExtensions
{
    public static void RegisterPipes(this IServiceCollection services)
    {
        services.AddMessagePipe();
        //var host = Dns.GetHostEntry("magictserver");
        //var hostIp = host.AddressList.Last().ToString();
        //services.AddMessagePipeTcpInterprocess(hostIp, 5029, x => x.InstanceLifetime = InstanceLifetime.Singleton);

        services.AddMessagePipeTcpInterprocess("127.0.0.1", 5029, x => x.InstanceLifetime = InstanceLifetime.Singleton);

    }
}
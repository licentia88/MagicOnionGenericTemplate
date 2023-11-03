using MessagePipe;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Shared.Extensions;

public static class MessagePipeExtensions
{
    public static void RegisterPipes(this IServiceCollection services)
    {
        services.AddMessagePipe();
        services.AddMessagePipeTcpInterprocess("127.0.0.1", 3215, x=> x.InstanceLifetime= InstanceLifetime.Singleton ); //


    }
}
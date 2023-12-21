using MessagePipe;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Shared.Extensions;

public static class MessagePipeExtensions
{
    public static void RegisterPipes(this IServiceCollection services)
    {
        services.AddMessagePipe();
        
        services.AddMessagePipeTcpInterprocess("magictweb", 3214, x=> x.InstanceLifetime= InstanceLifetime.Singleton ); //
        //services.AddMessagePipeNamedPipeInterprocess("foobar", x=> x.InstanceLifetime = InstanceLifetime.Singleton);
        //services.AddMessagePipeNamedPipeInterprocess("foobar2", x => x.InstanceLifetime = InstanceLifetime.Singleton);


    }
}
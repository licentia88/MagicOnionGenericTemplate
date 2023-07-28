using Magic.Client.Hubs;
using Microsoft.Extensions.DependencyInjection;

namespace Magic.Client.Extensions;

public static class DependencyExtensions
{
    public static void RegisterClientServices(this IServiceCollection Services)
    {
        Services.AutoRegisterFromMagicClient();
        Services.AddSingleton<TestHub>();
        Services.AddSingleton(typeof(List<>));
    }
}


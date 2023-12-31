using Blazored.LocalStorage;
using MagicT.Client.Hubs;
using MagicT.Redis.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Extensions;

/// <summary>
/// Extension methods for registering client services.
/// </summary>
public static class DependencyExtensions
{
    /// <summary>
    /// Registers client services in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public static void RegisterClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AutoRegister();

        services.AutoRegisterFromMagicTShared();

        services.AddSingleton(typeof(List<>));

        services.AddSingleton(typeof(Lazy<>));

        services.AddBlazoredLocalStorage();

        // Register Redis database services based on configuration.
        services.RegisterRedisDatabase();

    }
    //private static void RegisterHubsAndServices(this IServiceCollection services)
    //{
    //    // Add the TestHub singleton for SignalR communication.
    //    services.AddSingleton<TestHub>();
    //}
}
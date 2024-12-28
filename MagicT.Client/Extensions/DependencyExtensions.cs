using System.Text.Json;
// using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Coravel;
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

        // services.AddBlazoredLocalStorage();

        services.AddBlazoredSessionStorage(config => {
                config.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                config.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
                config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                config.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                config.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                config.JsonSerializerOptions.WriteIndented = false;
            }
        );
        // Register Redis database services based on configuration.
        services.RegisterRedisDatabase();

        services.AddScheduler();
    }
   
}
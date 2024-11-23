using MagicT.Redis.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Redis.Extensions;

/// <summary>
/// Provides extension methods for registering Redis-related services in the dependency injection container.
/// </summary>
public static class DependencyExtensions
{
    /// <summary>
    /// Registers the Redis database and related services in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    public static void RegisterRedisDatabase(this IServiceCollection services)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.InstanceName = "MagicTRedisInstance";
        });

     
    }
}
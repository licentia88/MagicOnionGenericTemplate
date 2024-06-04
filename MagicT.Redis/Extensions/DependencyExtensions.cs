using MagicT.Redis.Services;
using MemoryPack;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Redis.Extensions;

public static class SerializerExtensions
{
    public static byte[] SerializeToBytes<T>(this T obj)
    {
        var serializedBytes = MemoryPackSerializer.Serialize(obj);
        return serializedBytes;
    }

    public static T DeserializeFromBytes<T>(this byte[] bytes)
    {
        if (bytes is null) return default;

        return MemoryPackSerializer.Deserialize<T>(bytes);
    }
}


public static class DependencyExtensions
{
    public static void RegisterRedisDatabase(this IServiceCollection services)
    {

        services.AddStackExchangeRedisCache(options =>
        {
            options.InstanceName = "MagicTRedisInstance";
        });

        // Inject MagicTRedisDatabase as a singleton
        services.AddSingleton<MagicTRedisDatabase>();

        // services.AddSingleton<MagicTRedisDatabase>();

        // Inject RateLimiter as a singleton service
        services.AddSingleton<RateLimiterService>();

        // Inject ClientBlocker as a singleton service
        services.AddSingleton<ClientBlockerService>();

        services.AddSingleton<TokenCacheService>();
 
    }
}
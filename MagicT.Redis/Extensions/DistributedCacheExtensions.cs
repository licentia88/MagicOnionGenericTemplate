using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;

namespace MagicT.Redis.Extensions;

//TODO:Implement a faster serializer such as msgpack or memorypack
public static class DistributedCacheExtensions
{
    private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = null,
        WriteIndented = true,
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static Task SetAsync<T>(this IDistributedCache cache, string key, T value)
    {
        return SetAsync(cache, key, value, new DistributedCacheEntryOptions());
    }

    public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options)
    {
        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, GetJsonSerializerOptions()));
        return cache.SetAsync(key, bytes, options);
    }

    public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key)
    {
        var val = await cache.GetAsync(key);

        if (val == null) return default;

       return JsonSerializer.Deserialize<T>(val, GetJsonSerializerOptions());
    }

    public static bool TryGetValue<T>(this IDistributedCache cache, string key, out T value)
    {
        var val = cache.Get(key);
        value = default;

        if (val == null) return false;

        value = JsonSerializer.Deserialize<T>(val, GetJsonSerializerOptions());

        return true;
    }

    private static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return _serializerOptions;
    }
}

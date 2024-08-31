using MessagePipe.Redis;

namespace MagicT.Shared.Serializers;

/// <summary>
/// Provides serialization and deserialization methods for Redis using MemoryPack.
/// </summary>
public class RedisMemoryPackSerializer : IRedisSerializer
{
    /// <summary>
    /// Deserializes the specified byte array to an object of type T.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="value">The byte array to deserialize.</param>
    /// <returns>The deserialized object of type T.</returns>
    public T Deserialize<T>(byte[] value)
    {
        return MemoryPackSerializer.Deserialize<T>(value);
    }

    /// <summary>
    /// Serializes the specified object to a byte array.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="value">The object to serialize.</param>
    /// <returns>The serialized byte array.</returns>
    public byte[] Serialize<T>(T value)
    {
        var serializedBytes = MemoryPackSerializer.Serialize(value);
        return serializedBytes;
    }
}
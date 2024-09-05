using MemoryPack;

namespace MagicT.Redis.Extensions;

/// <summary>
/// Provides extension methods for serializing and deserializing objects to and from byte arrays.
/// </summary>
public static class SerializerExtensions
{
    /// <summary>
    /// Serializes an object to a byte array.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>A byte array representing the serialized object.</returns>
    public static byte[] SerializeToBytes<T>(this T obj)
    {
        var serializedBytes = MemoryPackSerializer.Serialize(obj);
        return serializedBytes;
    }

    /// <summary>
    /// Deserializes a byte array to an object.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="bytes">The byte array to deserialize.</param>
    /// <returns>The deserialized object, or the default value of <typeparamref name="T"/> if the byte array is null.</returns>
    public static T DeserializeFromBytes<T>(this byte[] bytes)
    {
        if (bytes is null) return default;

        return MemoryPackSerializer.Deserialize<T>(bytes);
    }
}
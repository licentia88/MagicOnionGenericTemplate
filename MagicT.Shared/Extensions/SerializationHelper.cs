namespace MagicT.Shared.Extensions;


public static class SerializerExtensions
{
    //private static BinarySerializer serializer = FsPickler.CreateBinarySerializer();
 
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

    public static ReadOnlySpan<byte> SerializeToSpanBytes<T>(this T obj)
    {
        var serializedBytes = MemoryPackSerializer.Serialize(obj);
        return serializedBytes.AsSpan();
    }

    public static T DeserializeFromSpanBytes<T>(this ReadOnlySpan<byte> bytes)
    {
        if (bytes.IsEmpty) return default;

        return MemoryPackSerializer.Deserialize<T>(bytes.ToArray());
    }
    
}

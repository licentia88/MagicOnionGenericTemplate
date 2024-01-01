using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using MBrace.FsPickler;

using MemoryPack;

namespace MagicT.Shared.Extensions;


public static class SerializerExtensions
{
    private static BinarySerializer serializer = FsPickler.CreateBinarySerializer();

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

    public static byte[] PickleToBytes<T>(this T obj)
    {
        return serializer.Pickle(obj);
    }

    public static T UnPickleFromBytes<T>(this byte[] bytes)
    {
        if (bytes is null) return default;
        return serializer.UnPickle<T>(bytes);
    }
}

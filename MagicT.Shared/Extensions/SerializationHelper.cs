using MemoryPack;

namespace MagicT.Shared.Extensions;

public static class SerializationHelper
{

    public static byte[] SerializeToBytes<T>(this T obj)
    {
       return MemoryPackSerializer.Serialize(obj);
    }

    public static T DeserializeFromBytes<T>(this byte[] bytes)
    {
        return MemoryPackSerializer.Deserialize<T>(bytes);
    }
}

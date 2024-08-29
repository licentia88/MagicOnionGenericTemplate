using MessagePipe.Redis;

namespace MagicT.Shared.Serializers;

public class RedisMemoryPackSerializer : IRedisSerializer
{
    public T Deserialize<T>(byte[] value)
    {
        return MemoryPackSerializer.Deserialize<T>(value);
    }

    public byte[] Serialize<T>(T value)
    {
        var serializedBytes = MemoryPackSerializer.Serialize(value);
        return serializedBytes;
    }
}


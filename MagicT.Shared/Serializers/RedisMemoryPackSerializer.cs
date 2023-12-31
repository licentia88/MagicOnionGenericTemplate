using MagicT.Shared.Extensions;
using MessagePipe.Redis;

namespace MagicT.Shared.Serializers;

public class RedisMemoryPackSerializer : IRedisSerializer
{
    public T Deserialize<T>(byte[] value)
    {
        return value.DeserializeFromBytes<T>();
    }

    public byte[] Serialize<T>(T value)
    {
        return value.SerializeToBytes();
    }
}


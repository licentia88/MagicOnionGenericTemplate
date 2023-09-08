using MagicT.Shared.Extensions;
using Tenray.ZoneTree.Serializers;

namespace MagicT.Server.ZoneTree.Serializers;

public sealed class ZoneTreeSerializer<TModel> : ISerializer<TModel> where TModel:class
{
    public TModel Deserialize(byte[] bytes)
    {
        return bytes.DeserializeFromBytes<TModel>();
    }

    public byte[] Serialize(in TModel entry)
    {
        return entry.SerializeToBytes();

     }

    //public string Deserialize(byte[] bytes)
    //{
    //    if (bytes.Length == 1 && bytes[0] == 194)
    //    {
    //        return null;
    //    }
    //    return Encoding.UTF8.GetString(bytes);
    //}

    //public byte[] Serialize(in string entry)
    //{
    //    if (entry == null)
    //    {
    //        return new byte[1] { 194 };
    //    }
    //    return Encoding.UTF8.GetBytes(entry);
    //}

    //byte[] ISerializer<string>.Serialize(in string entry)
    //{
    //    return Serialize(in entry);
    //}
}

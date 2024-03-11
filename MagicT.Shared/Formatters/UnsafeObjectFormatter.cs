using MemoryPack;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace MagicT.Shared.Formatters;

public sealed class UnsafeObjectFormatter : MemoryPackFormatter<object>
{
    public static readonly UnsafeObjectFormatter Default = new UnsafeObjectFormatter();

    // see:http://msdn.microsoft.com/en-us/library/w3f99sx1.aspx
    static readonly Regex AssemblyNameVersionSelectorRegex = new Regex(@", Version=\d+.\d+.\d+.\d+, Culture=[\w-]+, PublicKeyToken=(?:null|[a-f0-9]{16})", RegexOptions.Compiled);
    static readonly ConcurrentDictionary<Type, string> typeNameCache = new ConcurrentDictionary<Type, string>();

    public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref object? value)
    {
        if (value == null || value is DBNull|| value is MessagePack.Nil)
        {
            writer.WriteNullObjectHeader();
            return;
        }

        var type = value.GetType();

        if (!typeNameCache.TryGetValue(type, out var typeName))
        {
            var full = type.AssemblyQualifiedName!;

            var shortened = AssemblyNameVersionSelectorRegex.Replace(full, string.Empty);
            if (Type.GetType(shortened, false) == null)
            {
                // if type cannot be found with shortened name - use full name
                shortened = full;
            }

            typeNameCache[type] = shortened;
            typeName = shortened;
        }

        writer.WriteObjectHeader(2);
        writer.WriteString(typeName);
        writer.WriteValue(type, value);
    }

    public override void Deserialize(ref MemoryPackReader reader, scoped ref object? value)
    {
        if (!reader.TryReadObjectHeader(out var count))
        {
            value = null;
            return;
        }

        if (count != 2) MemoryPackSerializationException.ThrowInvalidPropertyCount(2, count);

        var typeName = reader.ReadString();
        var type = Type.GetType(typeName!);
        reader.ReadValue(type!, ref value);
    }
}


using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace MagicT.Shared.Formatters;

/// <summary>
/// Provides a formatter for serializing and deserializing objects with unsafe operations.
/// </summary>
public sealed class UnsafeObjectFormatter : MemoryPackFormatter<object>
{
    // see:http://msdn.microsoft.com/en-us/library/w3f99sx1.aspx
    /// <summary>
    /// A regex to match and remove assembly version, culture, and public key token from type names.
    /// </summary>
    static readonly Regex AssemblyNameVersionSelectorRegex = new Regex(@", Version=\d+.\d+.\d+.\d+, Culture=[\w-]+, PublicKeyToken=(?:null|[a-f0-9]{16})", RegexOptions.Compiled);

    /// <summary>
    /// A cache for storing shortened type names.
    /// </summary>
    static readonly ConcurrentDictionary<Type, string> TypeNameCache = new();

    /// <summary>
    /// Serializes the specified object to the provided writer.
    /// </summary>
    /// <typeparam name="TBufferWriter">The type of the buffer writer.</typeparam>
    /// <param name="writer">The writer to serialize the object to.</param>
    /// <param name="value">The object to serialize.</param>
    public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref object value)
    {
        if (value == null || value is DBNull || value is MessagePack.Nil)
        {
            writer.WriteNullObjectHeader();
            return;
        }

        var type = value.GetType();

        if (!TypeNameCache.TryGetValue(type, out var typeName))
        {
            var full = type.AssemblyQualifiedName!;

            var shortened = AssemblyNameVersionSelectorRegex.Replace(full, string.Empty);
            if (Type.GetType(shortened, false) == null)
            {
                // if type cannot be found with shortened name - use full name
                shortened = full;
            }

            TypeNameCache[type] = shortened;
            typeName = shortened;
        }

        writer.WriteObjectHeader(2);
        writer.WriteString(typeName);
        writer.WriteValue(type, value);
    }

    /// <summary>
    /// Deserializes an object from the provided reader.
    /// </summary>
    /// <param name="reader">The reader to deserialize the object from.</param>
    /// <param name="value">The deserialized object.</param>
    public override void Deserialize(ref MemoryPackReader reader, scoped ref object value)
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
using StructureOfArraysGenerator;

namespace MagicT.Shared.Formatters;

/// <summary>
/// Formatter for serializing and deserializing multi-dimensional arrays.
/// </summary>
/// <typeparam name="T">The type of the elements in the multi-dimensional array, which must be a struct and implement IMultiArray&lt;T&gt;.</typeparam>
internal sealed class MultiArrayFormatter<T> : MemoryPackFormatter<T>
    where T : struct, IMultiArray<T>
{
    /// <summary>
    /// Serializes the multi-dimensional array.
    /// </summary>
    /// <typeparam name="TBufferWriter">The type of the buffer writer.</typeparam>
    /// <param name="writer">The writer to which the data will be serialized.</param>
    /// <param name="value">The multi-dimensional array to serialize.</param>
    public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref T value)
    {
        writer.WriteUnmanaged(value.Length);
        writer.WriteUnmanagedSpan(value.GetRawSpan());
    }

    /// <summary>
    /// Deserializes the multi-dimensional array.
    /// </summary>
    /// <param name="reader">The reader from which the data will be deserialized.</param>
    /// <param name="value">The multi-dimensional array to deserialize.</param>
    public override void Deserialize(ref MemoryPackReader reader, scoped ref T value)
    {
        var length = reader.ReadUnmanaged<int>();
        var array = reader.ReadUnmanagedArray<byte>();
        value = T.Create(length, array!);
    }
}
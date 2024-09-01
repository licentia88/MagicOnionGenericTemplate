using Grpc.Core;

namespace MagicT.Client.Extensions;

/// <summary>
/// Extension methods for the service context.
/// </summary>
public static class ServiceContextExtensions
{
    /// <summary>
    /// Adds or updates a string item in the metadata headers.
    /// </summary>
    /// <param name="headers">The metadata headers.</param>
    /// <param name="key">The key of the item.</param>
    /// <param name="value">The value of the item.</param>
    public static void AddOrUpdateItem(this Metadata headers, string key, string value)
    {
        var existingEntry = headers.FirstOrDefault(x => x.Key == key);

        if (existingEntry is null)
        {
            headers.Add(key, value);
        }
        else
        {
            headers.Remove(existingEntry);
            headers.Add(key, value);
        }
    }

    /// <summary>
    /// Adds or updates a byte array item in the metadata headers.
    /// </summary>
    /// <param name="headers">The metadata headers.</param>
    /// <param name="key">The key of the item.</param>
    /// <param name="value">The value of the item.</param>
    public static void AddOrUpdateItem(this Metadata headers, string key, byte[] value)
    {
        if (value is null) return;

        var existingEntry = headers.FirstOrDefault(x => x.Key == key);

        if (existingEntry is null)
        {
            headers.Add(key, value);
        }
        else if (!existingEntry.ValueBytes.SequenceEqual(value))
        {
            headers.Remove(existingEntry);
            headers.Add(key, value);
        }
    }
}
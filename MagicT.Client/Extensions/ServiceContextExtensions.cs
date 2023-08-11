using Grpc.Core;

namespace MagicT.Client.Extensions;

/// <summary>
/// Extension methods for the service context.
/// </summary>
public static class ServiceContextExtensions
{
    /// <summary>
    /// Registers client services in the service context.
    /// </summary>
    /// <param name="headers"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void AddorUpdateItem(this Metadata headers, string key, string value) 
    {
        var existingEntry = headers.FirstOrDefault(x => x.Key == key);

        if (existingEntry is null)
        {
            headers.Add(key,value);
            return;
        }

        headers.Remove(existingEntry);
        
        headers.Add(key,value);
    } 
    
    /// <summary>
    /// Registers client services in the service context.
    /// </summary>
    /// <param name="headers"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void AddorUpdateItem(this Metadata headers, string key, byte[] value) 
    {
        var existingEntry = headers.FirstOrDefault(x => x.Key == key);
        
        if (existingEntry is null)
        {
            headers.Add(key,value);
            return;
        }
        
        if(existingEntry.ValueBytes.Equals(value)) return;

        headers.Remove(existingEntry);
        
        headers.Add(key,value);
    } 
}
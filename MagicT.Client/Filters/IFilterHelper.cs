namespace MagicT.Client.Filters;

/// <summary>
/// Interface for creating authentication headers.
/// </summary>
public interface IFilterHelper
{
    /// <summary>
    /// Creates the authentication header data.
    /// </summary>
    /// <returns>A tuple containing the header key and data.</returns>
    ValueTask<(string Key, byte[] Data)> CreateHeaderAsync();
}
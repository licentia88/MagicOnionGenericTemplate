using MagicOnion.Client;

namespace MagicT.Client.Extensions;

/// <summary>
/// Provides extension methods for types.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Gets the first filter of the specified type from the array of filters.
    /// </summary>
    /// <typeparam name="TFilter">The type of filter to get.</typeparam>
    /// <param name="filters">The array of filters.</param>
    /// <returns>The first filter of the specified type, or null if no such filter is found.</returns>
    public static TFilter Get<TFilter>(this IClientFilter[] filters) where TFilter : IClientFilter
    {
        return filters.OfType<TFilter>().FirstOrDefault();
    }
}
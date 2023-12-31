using MagicOnion.Client;

namespace MagicT.Client.Extensions;

public static class TypeExtensions
{
    public static TFilter Get<TFilter>(this IClientFilter[] Filters) where TFilter : IClientFilter
    {
        return Filters.OfType<TFilter>().FirstOrDefault();
    }
}

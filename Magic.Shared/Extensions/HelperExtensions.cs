using Mapster;

namespace Magic.Shared.Extensions;

public static class HelperExtensions
{
    public static TModel ToModel<TModel>(this object obj)
    {
        return obj.Adapt<TModel>();
    }
}

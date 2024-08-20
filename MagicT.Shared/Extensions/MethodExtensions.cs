using System.Reflection;

namespace MagicT.Shared.Extensions;

public static class MethodExtensions
{
    public static MethodInfo GetMethod(Type type, string methodName)
    {
        return type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }
}

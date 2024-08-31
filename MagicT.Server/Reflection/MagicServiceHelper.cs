using System.Reflection;
using MagicOnion;
// ReSharper disable UnusedMember.Local

namespace MagicT.Server.Reflection;

/// <summary>
/// Provides helper methods for finding MagicOnion service types and methods.
/// </summary>
public static class MagicServiceHelper
{
    /// <summary>
    /// Finds all MagicOnion service types in the current AppDomain.
    /// </summary>
    /// <returns>An enumerable of MagicOnion service types.</returns>
    public static IEnumerable<Type> FindMagicServiceTypes()
    {
        var baseType = typeof(IService<>);

        var services = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t != baseType &&
                        t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == baseType) &&
                        !t.IsAbstract)
            .ToList();

        return services;
    }

    /// <summary>
    /// Finds all MagicOnion service methods in the specified service type.
    /// </summary>
    /// <param name="serviceType">The service type to search for methods.</param>
    /// <returns>An enumerable of methods in the specified service type.</returns>
    public static IEnumerable<MethodInfo> FindMagicServiceMethods(Type serviceType)
    {
        var methods = serviceType.GetMethods()
            .Where(m =>
                m.ReturnType == typeof(UnaryResult<>) ||
                m.ReturnType == typeof(UnaryResult) ||
                m.ReturnType == typeof(Task<>) ||
                 (m.ReturnType is { IsGenericType: true, ContainsGenericParameters: false } &&
                  (m.ReturnType.GetGenericTypeDefinition() == typeof(UnaryResult<>) ||
                   m.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)))
            ).ToList();

        return methods;
    }

    /// <summary>
    /// Determines if the specified type has the specified generic base type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="genericBaseType">The generic base type to check for.</param>
    /// <returns>True if the type has the specified generic base type; otherwise, false.</returns>
    private static bool HasBaseType(Type type, Type genericBaseType)
    {
        while (type != null)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericBaseType)
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }
}
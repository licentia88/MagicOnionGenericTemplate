using MagicOnion;
using System.Reflection;

namespace MagicT.Server.Reflection;

public static class MagicServiceHelper
{
    public static IEnumerable<Type> FindMagicServiceTypes()
    {
        var baseType = typeof(IService<>);

 
        var services =  AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t != baseType &&
                        t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == baseType) &&
                        !t.IsAbstract)
            .ToList();

        return services;
    }

    public static IEnumerable<MethodInfo> FindMagicServiceMethods(Type serviceType) 
    {
        var methods = serviceType.GetMethods()
            .Where(m =>
                m.ReturnType == typeof(UnaryResult<>) ||
                m.ReturnType == typeof(UnaryResult) ||
                m.ReturnType == typeof(Task<>) ||
                 (m.ReturnType.IsGenericType && !m.ReturnType.ContainsGenericParameters &&
                  (m.ReturnType.GetGenericTypeDefinition() == typeof(UnaryResult<>) ||
                  m.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)))
            ).ToList();

        return methods;
    }

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

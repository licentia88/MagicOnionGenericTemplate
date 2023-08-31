using MagicOnion;
using MagicT.Server.Services.Base;
using MagicT.Shared.Services.Base;
using System.Reflection;

namespace MagicT.Server.Reflection;

public static class MagicServiceHelper
{
    public static IEnumerable<Type> FindMagicServiceTypes()
    {
        var baseType = typeof(MagicServerServiceBase<,,>);

        return AppDomain.CurrentDomain.GetAssemblies()
          .SelectMany(a => a.GetTypes())
          .Where(t => HasBaseType(t, baseType) && !t.IsAbstract).ToList();
    }

    public static IEnumerable<MethodInfo> FindMagicServiceMethods(Type serviceType) 
    {
        var methods = serviceType.GetMethods()
            .Where(m =>
                m.ReturnType == typeof(UnaryResult<>) ||
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

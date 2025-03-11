using System.ComponentModel;
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
    /// Finds all MagicOnion service types in the current AppDomain and retrieves their associated <see cref="DescriptionAttribute"/> value.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of tuples, where each tuple contains:
    /// - A <see cref="Type"/> object representing the MagicOnion service type.
    /// - A <see cref="string"/> representing the value of the <see cref="DescriptionAttribute"/> applied to the type,
    ///   or "No description" if the attribute is not present.
    /// </returns>
    public static IEnumerable<(Type ServiceType, string Description)> FindMagicServiceTypes()
    {
        var baseType = typeof(IService<>);
        var services = new List<(Type, string)>();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                var types = assembly.GetTypes()
                    .Where(t => t != baseType &&
                                t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == baseType) &&
                                !t.IsAbstract)
                    .Select(t =>
                    {
                        // Get the Description attribute value
                        var descriptionAttribute = t.GetCustomAttribute<DescriptionAttribute>();
                        var description = descriptionAttribute?.Description ?? "No description";
                        return (ServiceType: t, Description: description);
                    });

                services.AddRange(types);
            }
            catch (ReflectionTypeLoadException ex)
            {
                // Log the loader exceptions
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    Console.WriteLine(loaderException);
                }
            }
        }

        return services;
    }

    
    /// <summary>
    /// Finds methods in the specified service type that return either <see cref="UnaryResult"/>, <see cref="UnaryResult{T}"/>,
    /// <see cref="Task"/>, or <see cref="Task{TResult}"/>, and retrieves their associated <see cref="DescriptionAttribute"/> value.
    /// </summary>
    /// <param name="serviceType">The <see cref="Type"/> of the service to search for methods.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of tuples, where each tuple contains:
    /// - A <see cref="MethodInfo"/> object representing the method.
    /// - A <see cref="string"/> representing the value of the <see cref="DescriptionAttribute"/> applied to the method,
    ///   or "No description" if the attribute is not present.
    /// </returns>
    public static IEnumerable<(MethodInfo Method, string Description)> FindMagicServiceMethods(Type serviceType)
    {
        var methods = serviceType.GetMethods()
            .Where(m =>
            {
                var returnType = m.ReturnType;

                // Check for non-generic UnaryResult
                if (returnType == typeof(UnaryResult))
                    return true;

                // Check for generic UnaryResult<> or Task<>
                if (returnType is { IsGenericType: true, ContainsGenericParameters: false })
                {
                    var genericTypeDefinition = returnType.GetGenericTypeDefinition();
                    return genericTypeDefinition == typeof(UnaryResult<>) ||
                           genericTypeDefinition == typeof(Task<>);
                }

                return false;
            })
            .Select(m =>
            {
                // Get the Description attribute value
                var descriptionAttribute = m.GetCustomAttribute<DescriptionAttribute>();
                var description = descriptionAttribute?.Description ?? "No description";
                return (Method: m, Description: description);
            })
            .ToList();

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

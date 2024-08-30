using System.Reflection;
using MagicT.Shared.Models.ServiceModels;
// ReSharper disable UnusedMember.Global

namespace MagicT.Server.Extensions;

/// <summary>
/// Provides extension methods for reflection-related operations.
/// </summary>
// ReSharper disable once UnusedType.Global
public static class ReflectionExtensions
{
    /// <summary>
    /// Determines whether the specified method has a parameter of type <see cref="EncryptedData{T}"/>.
    /// </summary>
    /// <param name="methodInfo">The method information.</param>
    /// <returns><c>true</c> if the method has a parameter of type <see cref="EncryptedData{T}"/>; otherwise, <c>false</c>.</returns>
    public static bool IsEncryptedData(this MethodInfo methodInfo) => methodInfo.GetParameters()
        .Any(arg => arg.ParameterType.IsGenericType &&
                    arg.ParameterType.GetGenericTypeDefinition() == typeof(EncryptedData<>));

    /// <summary>
    /// Determines whether the specified method has a parameter of type <see cref="byte[]"/>.
    /// </summary>
    /// <param name="methodInfo">The method information.</param>
    /// <returns><c>true</c> if the method has a parameter of type <see cref="byte[]"/>; otherwise, <c>false</c>.</returns>
    public static bool IsByteArray(this MethodInfo methodInfo) => methodInfo.GetParameters()
        .Any(arg => arg.ParameterType == typeof(byte[]));
}
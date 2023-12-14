using System.Reflection;
using MagicT.Shared.Models.ServiceModels;

namespace MagicT.Server.Extensions;

public static class ReflectionExtensions
{
    public static bool IsEncryptedData(this MethodInfo methodInfo) => methodInfo.GetParameters()
             .Any((ParameterInfo arg) => arg.ParameterType.IsGenericType &&
                                                             arg.ParameterType.GetGenericTypeDefinition() == typeof(EncryptedData<>));

    public static bool IsByteArray(this MethodInfo methodInfo) => methodInfo.GetParameters()
             .Any((ParameterInfo arg) => arg.ParameterType == typeof(byte[]));
}

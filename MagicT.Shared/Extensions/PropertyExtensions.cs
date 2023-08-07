using System.Dynamic;
using System.Reflection;

namespace MagicT.Shared.Extensions;

public static class PropertyExtensions
{
    public static object GetPropertyValue<T>(this T obj, string propertyName)
    {
        if (obj is null) return default;

        if (obj is ExpandoObject || obj is Dictionary<string, object>)
            return ((IDictionary<string, object>) obj)[propertyName];

        return obj.GetType()
            .GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
            ?.GetValue(obj);
    }

    public static void SetPropertyValue<T>(this T obj, string propertyName, object propertyValue)
    {
        if (obj is ExpandoObject || obj is Dictionary<string, object>)
        {
            ((IDictionary<string, object>) obj)[propertyName] = propertyValue;

            return;
        }

        var property = obj.GetType()
            .GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

        if (property != null)
            property.SetValue(obj, ChangeToType(propertyValue, property.PropertyType));
    }

    private static object ChangeToType(object value, Type destinationType)
    {
        try
        {
            return Convert.ChangeType(value, Nullable.GetUnderlyingType(destinationType) ?? destinationType);
        }
        catch
        {
            return GetDefaultValue(destinationType);
        }
    }

    public static object GetFieldValue<T>(this T obj, string propertyName)
    {
        if (obj is null) return default;

        if (obj is ExpandoObject || obj is Dictionary<string, object>)
            return ((IDictionary<string, object>) obj)[propertyName];

        return obj.GetType()
            .GetField(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(obj);
    }

    public static void SetFieldValue<T>(this T obj, string propertyName, object propertyValue)
    {
        if (obj is ExpandoObject || obj is Dictionary<string, object>)
        {
            ((IDictionary<string, object>) obj)[propertyName] = propertyValue;

            return;
        }

        obj.GetType().GetField(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(obj, propertyValue);
    }

    public static object GetDefaultValue(this Type type)
    {
        if (type is not null && type.IsValueType && Nullable.GetUnderlyingType(type) == null)
            return Activator.CreateInstance(type);
        return null;
    }
}
using System.Dynamic;
using System.Reflection;

namespace MagicT.Shared.Extensions;

/// <summary>
/// Extensions for accessing property and field values dynamically.
/// </summary>
public static class PropertyExtensions
{
    /// <summary>
    /// Get the value of a property from an object dynamically.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object to get the property value from.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The value of the property.</returns>
    public static object GetPropertyValue<T>(this T obj, string propertyName)
    {
        if (obj is null) return default;

        // Check if the object is of type ExpandoObject or Dictionary<string, object>
        // and directly access the property value using dictionary indexing.
        if (obj is ExpandoObject || obj is Dictionary<string, object>)
            return ((IDictionary<string, object>)obj)[propertyName];

        // Use reflection to get the property value for other object types.
        return obj.GetType()
            .GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
            ?.GetValue(obj);
    }

    /// <summary>
    /// Set the value of a property on an object dynamically.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object to set the property value on.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="propertyValue">The value to set.</param>
    public static void SetPropertyValue<T>(this T obj, string propertyName, object propertyValue)
    {
        if (obj is ExpandoObject || obj is Dictionary<string, object>)
        {
            // If the object is of type ExpandoObject or Dictionary<string, object>,
            // directly set the property value using dictionary indexing.
            ((IDictionary<string, object>)obj)[propertyName] = propertyValue;
            return;
        }

        // Use reflection to set the property value for other object types.
        var property = obj.GetType()
            .GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

        if (property != null)
            property.SetValue(obj, ChangeToType(propertyValue, property.PropertyType));
    }

    /// <summary>
    /// Change the given value to the specified destination type.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <param name="destinationType">The destination type.</param>
    /// <returns>The converted value.</returns>
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

    /// <summary>
    /// Get the value of a field from an object dynamically.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object to get the field value from.</param>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>The value of the field.</returns>
    public static object GetFieldValue<T>(this T obj, string fieldName)
    {
        if (obj is null) return default;

        // Check if the object is of type ExpandoObject or Dictionary<string, object>
        // and directly access the field value using dictionary indexing.
        if (obj is ExpandoObject || obj is Dictionary<string, object>)
            return ((IDictionary<string, object>)obj)[fieldName];

        // Use reflection to get the field value for other object types.
        return obj.GetType()
            .GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(obj);
    }

    /// <summary>
    /// Set the value of a field on an object dynamically.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object to set the field value on.</param>
    /// <param name="fieldName">The name of the field.</param>
    /// <param name="fieldValue">The value to set.</param>
    public static void SetFieldValue<T>(this T obj, string fieldName, object fieldValue)
    {
        if (obj is ExpandoObject || obj is Dictionary<string, object>)
        {
            // If the object is of type ExpandoObject or Dictionary<string, object>,
            // directly set the field value using dictionary indexing.
            ((IDictionary<string, object>)obj)[fieldName] = fieldValue;
            return;
        }

        // Use reflection to set the field value for other object types.
        obj.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(obj, fieldValue);
    }

    /// <summary>
    /// Get the default value for the specified type.
    /// </summary>
    /// <param name="type">The type to get the default value for.</param>
    /// <returns>The default value for the type.</returns>
    public static object GetDefaultValue(this Type type)
    {
        if (type is not null && type.IsValueType && Nullable.GetUnderlyingType(type) == null)
            return Activator.CreateInstance(type);
        return null;
    }
}
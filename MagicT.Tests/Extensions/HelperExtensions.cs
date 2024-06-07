using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using FastDeepCloner;

namespace MagicT.Tests.Extensions;

public static class HelperExtensions
{
    private static readonly Random _random = new Random();

    public static T SetRandomProperty<T>(this T obj) where T : class
    {
        var cloned = obj.Clone<T>();

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite &&
                        !p.GetCustomAttributes<KeyAttribute>().Any() &&
                        !p.GetCustomAttributes<ForeignKeyAttribute>().Any() &&
                        IsSupportedType(p.PropertyType))
            .ToList();

        if (properties.Count == 0)
        {
            throw new InvalidOperationException("No properties available to set a random value.");
        }

        var propertyToSet = properties[_random.Next(properties.Count)];
        object currentValue = propertyToSet.GetValue(cloned);
        object randomValue;

        do
        {
            randomValue = GenerateRandomValue(propertyToSet.PropertyType, currentValue);
        } while (Equals(currentValue, randomValue));

        propertyToSet.SetValue(cloned, randomValue);

        return cloned;
    }

    private static bool IsSupportedType(Type type)
    {
        // Exclude user-defined classes and collection types
        return type.IsPrimitive || type == typeof(string) || type == typeof(DateTime) ||
               (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
               IsSupportedType(Nullable.GetUnderlyingType(type)));
    }

    private static object GenerateRandomValue(Type type, object currentValue)
    {
        if (type == typeof(int))
        {
            int newValue;
            do
            {
                newValue = _random.Next(1, 100); // random int between 1 and 99
            } while ((int)currentValue == newValue);
            return newValue;
        }
        if (type == typeof(double))
        {
            double newValue;
            do
            {
                newValue = _random.NextDouble() * 100; // random double between 0 and 100
            } while (Math.Abs((double)currentValue - newValue) < 0.0001);
            return newValue;
        }
        if (type == typeof(string))
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string newValue;
            do
            {
                newValue = new string(Enumerable.Repeat(chars, 10)
                  .Select(s => s[_random.Next(s.Length)]).ToArray());
            } while ((string)currentValue == newValue);
            return newValue;
        }
        if (type == typeof(bool))
        {
            return !(bool)currentValue; // return the opposite value
        }
        if (type == typeof(DateTime))
        {
            DateTime newValue;
            do
            {
                newValue = DateTime.Now.AddDays(_random.Next(-1000, 1000));
            } while ((DateTime)currentValue == newValue);
            return newValue;
        }
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            Type underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType != null ? GenerateRandomValue(underlyingType, currentValue) : null;
        }

        throw new NotSupportedException($"Random value generation for type {type} is not supported.");
    }
}

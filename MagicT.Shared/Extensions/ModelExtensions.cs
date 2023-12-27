using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace MagicT.Shared.Extensions;

public static class ModelExtensions
{
    public static string GetPrimaryKey<TModel>()
    {
        var type = typeof(TModel);

        var primaryKeyProperty = type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
            .FirstOrDefault(p => Attribute.IsDefined(p, typeof(KeyAttribute)));

        if (primaryKeyProperty == null) throw new InvalidOperationException("Primary key not found for the model.");


        return primaryKeyProperty.Name;
    }

    public static string GetPrimaryKey(Type type)
    {
        var primaryKeyProperty = type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
            .FirstOrDefault(p => Attribute.IsDefined(p, typeof(KeyAttribute)));

        if (primaryKeyProperty == null) throw new InvalidOperationException("Primary key not found for the model.");


        return primaryKeyProperty.Name;
    }

    public static string GetPrimaryKey<TModel>(this TModel model)
    {
        var type = typeof(TModel);

        var primaryKeyProperty = type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
            .FirstOrDefault(p => Attribute.IsDefined(p, typeof(KeyAttribute)));

        if (primaryKeyProperty == null) throw new InvalidOperationException("Primary key not found for the model.");


        return primaryKeyProperty.Name;
    }

    public static string GetForeignKey<TModel, TChild>()
    {
        var modelType = typeof(TModel);
        var childType = typeof(TChild);

        var collectionProperty = modelType.GetProperties()
            .FirstOrDefault(p =>
                p.PropertyType.IsGenericType &&
                p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>) &&
                p.PropertyType.GetGenericArguments().Any(t => childType.IsAssignableFrom(t)));

        if (collectionProperty == null)
            throw new ArgumentException(
                $"TModel does not have a collection property of type ICollection<{childType.Name}>.");


        var foreignKeyAttribute = collectionProperty.GetCustomAttribute<ForeignKeyAttribute>();

        if (foreignKeyAttribute == null)
            throw new ArgumentException(
                "The collection property does not have a ForeignKey attribute.");

        return foreignKeyAttribute.Name;
    }
}

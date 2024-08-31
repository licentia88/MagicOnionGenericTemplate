using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace MagicT.Shared.Extensions;

/// <summary>
/// Provides extension methods for model classes.
/// </summary>
public static class ModelExtensions
{
    /// <summary>
    /// Gets the name of the primary key property for the specified model type.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <returns>The name of the primary key property.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the primary key is not found.</exception>
    public static string GetPrimaryKey<TModel>()
    {
        return GetPrimaryKey(typeof(TModel));
    }

    /// <summary>
    /// Gets the name of the primary key property for the specified model type.
    /// </summary>
    /// <param name="type">The type of the model.</param>
    /// <returns>The name of the primary key property.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the primary key is not found.</exception>
    public static string GetPrimaryKey(Type type)
    {
        var primaryKeyProperty = type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
            .FirstOrDefault(p => Attribute.IsDefined(p, typeof(KeyAttribute)));

        if (primaryKeyProperty == null) throw new InvalidOperationException("Primary key not found for the model.");

        return primaryKeyProperty.Name;
    }

    /// <summary>
    /// Gets the name of the primary key property for the specified model instance.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="model">The model instance.</param>
    /// <returns>The name of the primary key property.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the primary key is not found.</exception>
    public static string GetPrimaryKey<TModel>(this TModel model)
    {
        return GetPrimaryKey(typeof(TModel));
    }

    /// <summary>
    /// Gets the name of the foreign key property for the specified model and child types.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TChild">The type of the child model.</typeparam>
    /// <returns>The name of the foreign key property.</returns>
    /// <exception cref="ArgumentException">Thrown when the collection property or ForeignKey attribute is not found.</exception>
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
using Mapster;

namespace MagicT.Shared.Extensions;

/// <summary>
/// The MappingExtensions class provides a number of extension methods for mapping objects to and from different models.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Converts an object of any type to a model of a specified type. The model type must be a subclass of `BaseModel`.
    /// </summary>
    /// <typeparam name="TModel">The type of the model to create.</typeparam>
    /// <param name="obj">The object to convert.</param>
    /// <returns>A model of type `TModel`.</returns>
    /// <remarks>
    /// This method uses the `Mapster` library to perform the conversion.
    /// </remarks>
    public static TModel ToModel<TModel>(this object obj)
    {
        /// <remarks>
        /// This method is used to convert an object of any type to a model of a specified type.
        /// The model type must be a subclass of `BaseModel`.
        /// </remarks>
        return obj.Adapt<TModel>();
    }

    
}
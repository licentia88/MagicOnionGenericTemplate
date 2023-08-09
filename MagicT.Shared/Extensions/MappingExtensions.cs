using MagicT.Shared.Models.Base;
using MagicT.Shared.Models.MemoryDatabaseModels;
using Mapster;

/// <summary>
/// The MappingExtensions class provides a number of extension methods for mapping objects to and from different models.
/// </summary>
public static class MappingExtensions
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

    /// <summary>
    /// Converts an object of type `AUTHORIZATIONS_BASE` to a model of type `Authorizations`. The `Authorizations` model is a memory database model that is used to store authorization data.
    /// </summary>
    /// <typeparam name="TDest">The type of the destination model.</typeparam>
    /// <param name="source">The object to convert.</param>
    /// <returns>A model of type `TDest`.</returns>
    /// <remarks>
    /// This method uses the `Mapster` library to perform the conversion.
    /// </remarks>
    public static TDest MapToMemoryModel<TDest>(this AUTHORIZATIONS_BASE source) where TDest : Authorizations
    {
        /// <remarks>
        /// This method is used to convert an object of type `AUTHORIZATIONS_BASE` to a model of type `Authorizations`.
        /// The `Authorizations` model is a memory database model that is used to store authorization data.
        /// </remarks>
        var config = new TypeAdapterConfig();

        config.NewConfig<AUTHORIZATIONS_BASE, TDest>()
            .Map(dest => dest.Id, src => src.AB_ROWID)
            .Map(dest => dest.UserRefNo, src => src.AB_USER_REFNO)
            .Map(dest => dest.AuthType, src => src.AB_AUTH_TYPE)
            .Ignore(dest => dest.Description);

        return source.Adapt<TDest>(config);
    }

    /// <summary>
    /// Converts a list of objects of type `AUTHORIZATIONS_BASE` to a list of models of type `Authorizations`.
    /// </summary>
    /// <typeparam name="TDest">The type of the destination model.</typeparam>
    /// <param name="sourceList">The list of objects to convert.</param>
    /// <returns>A list of models of type `TDest`.</returns>
    /// <remarks>
    /// This method uses the `Mapster` library to perform the conversion.
    /// </remarks>
    public static List<TDest> MapToMemoryModelList<TDest>(this IEnumerable<AUTHORIZATIONS_BASE> sourceList) where TDest : Authorizations, new()
    {
        /// <remarks>
        /// This method is used to convert a list of objects of type `AUTHORIZATIONS_BASE` to a list of models of type `Authorizations`.
        /// </remarks>
        var config = new TypeAdapterConfig();

        config.NewConfig<AUTHORIZATIONS_BASE, TDest>()
            .Map(dest => dest.Id, src => src.AB_ROWID)
            .Map(dest => dest.UserRefNo, src => src.AB_USER_REFNO)
            .Map(dest => dest.AuthType, src => src.AB_AUTH_TYPE)
            .Ignore(dest => dest.Description);

        return sourceList.Adapt<List<TDest>>(config);
    }
}

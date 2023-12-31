using MagicOnion;

namespace MagicT.Shared.Services.Base;


/// <summary>
///     Represents a generic service with CRUD operations.
/// </summary>
/// <typeparam name="TService">The type of service.</typeparam>
/// <typeparam name="TModel">The type of model.</typeparam>
public interface IMagicService<TService, TModel> : IService<TService>  
{
    /// <summary>
    ///     Creates a new instance of the specified model.
    /// </summary>
    /// <param name="model">The model to create.</param>
    /// <returns>A unary result containing the created model.</returns>
    UnaryResult<TModel> CreateAsync(TModel model);


    /// <summary>
    ///     Retrieves a list of models based on the parent primaryKey request.
    /// </summary>
    /// <param name="parentId"></param>
    /// <param name="foreignKey"></param>
    /// <returns>A unary result containing a list of models.</returns>
    UnaryResult<List<TModel>> FindByParentAsync(string parentId, string foreignKey);


    /// <summary>
    ///  Retrieves a list of models based on given parameters
    /// </summary>
    /// <param name="parameterBytes"></param>
    /// <returns>A unary result containing a list of models.</returns>
    UnaryResult<List<TModel>> FindByParametersAsync(byte[] parameterBytes);


    /// <summary>
    ///     Retrieves all models.
    /// </summary>
    /// <returns>A unary result containing a list of all models.</returns>
    UnaryResult<List<TModel>> ReadAsync();

    /// <summary>
    ///     Retrieves all with batches.
    /// </summary>
    /// <returns>A unary result containing a list of all models.</returns>
    Task<ServerStreamingResult<List<TModel>>> StreamReadAllAsync(int batchSize);

    /// <summary>
    ///     Updates the specified model.
    /// </summary>
    /// <param name="model">The model to update.</param>
    /// <returns>A unary result containing the updated model.</returns>
    UnaryResult<TModel> UpdateAsync(TModel model);

    /// <summary>
    ///     Deletes the specified model.
    /// </summary>
    /// <param name="model">The model to delete.</param>
    /// <returns>A unary result containing the deleted model.</returns>
    UnaryResult<TModel> DeleteAsync(TModel model);

   

}


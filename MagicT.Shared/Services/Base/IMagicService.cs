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
    ///     Creates a new instance of the specified model asynchronously.
    /// </summary>
    /// <param name="model">The model to create.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the created model.</returns>
    UnaryResult<TModel> CreateAsync(TModel model);

    /// <summary>
    ///     Creates multiple instances of the specified model asynchronously.
    /// </summary>
    /// <param name="models">The list of models to create.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing a list of the created models.</returns>
    UnaryResult<List<TModel>> CreateRangeAsync(List<TModel> models);


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
    ///     Updates multiple instances of the specified model asynchronously.
    /// </summary>
    /// <param name="models">The list of models to update.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the updated models.</returns>
    UnaryResult<List<TModel>> UpdateRangeAsync(List<TModel> models);


    /// <summary>
    ///     Removes multiple instances of the specified model asynchronously.
    /// </summary>
    /// <param name="models">The list of models to remove.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> indicating the success of the removal operation.</returns>
    UnaryResult<List<TModel>> DeleteRangeAsync(List<TModel> models);

    /// <summary>
    ///     Deletes the specified model.
    /// </summary>
    /// <param name="model">The model to delete.</param>
    /// <returns>A unary result containing the deleted model.</returns>
    UnaryResult<TModel> DeleteAsync(TModel model);

   

}


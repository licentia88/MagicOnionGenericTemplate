using MagicOnion;
using MagicOnion.Client;
using MagicT.Shared.Services.Base;


namespace MagicT.Client.Services.Base;

/// <summary>
///     Abstract base class for a generic service implementation.
/// </summary>
/// <typeparam name="TService">The type of service.</typeparam>
/// <typeparam name="TModel">The type of model.</typeparam>
public abstract class MagicClientService<TService, TModel> : MagicClientServiceBase<TService>, IMagicService<TService, TModel>
    where TService : IMagicService<TService, TModel> 
{

    public IClientFilter[] Filters { get; set; } = default;

    protected MagicClientService(IServiceProvider provider) : base(provider)
    {
    }

    protected MagicClientService(IServiceProvider provider, params IClientFilter[] filters) : base(provider, filters)
    {
        Filters = filters;
    }



    /// <summary>
    ///     Creates a new instance of the specified model.
    /// </summary>
    /// <param name="model">The model to create.</param>
    /// <returns>A unary result containing the created model.</returns>
    public virtual UnaryResult<TModel> CreateAsync(TModel model)
    {
        return Client.CreateAsync(model);
    }

    /// <summary>
    ///     Creates multiple instances of the specified model asynchronously.
    /// </summary>
    /// <param name="models">The list of models to create.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing a list of the created models.</returns>
    public UnaryResult<List<TModel>> CreateRangeAsync(List<TModel> models)
    {
        return Client.CreateRangeAsync(models);
    }


    /// <summary>
    /// Retrieves a list of entities of type TModel associated with a parent entity based on a foreign key.
    /// </summary>
    /// <param name="parentId">The identifier of the parent entity.</param>
    /// <param name="foreignKey">The foreign key used to associate the entities with the parent entity.</param>
    /// <returns>A unary result containing the list of associated entities.</returns>
    public virtual UnaryResult<List<TModel>> FindByParentAsync(string parentId, string foreignKey)
    {
        return Client.FindByParentAsync(parentId, foreignKey);
    }


    /// <summary>
    ///     Updates the specified model.
    /// </summary>
    /// <param name="model">The model to update.</param>
    /// <returns>A unary result containing the updated model.</returns>
    public virtual UnaryResult<TModel> UpdateAsync(TModel model)
    {
        return Client.UpdateAsync(model);
    }

    /// <summary>
    ///     Updates multiple instances of the specified model asynchronously.
    /// </summary>
    /// <param name="models">The list of models to update.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the updated models.</returns>
    public UnaryResult<List<TModel>> UpdateRangeAsync(List<TModel> models)
    {
        return Client.UpdateRangeAsync(models);
    }

    /// <summary>
    ///     Deletes the specified model.
    /// </summary>
    /// <param name="model">The model to delete.</param>
    /// <returns>A unary result containing the deleted model.</returns>
    public virtual UnaryResult<TModel> DeleteAsync(TModel model)
    {
        return Client.DeleteAsync(model);
    }

    /// <summary>
    ///     Removes multiple instances of the specified model asynchronously.
    /// </summary>
    /// <param name="models">The list of models to remove.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> indicating the success of the removal operation.</returns>
    public UnaryResult<List<TModel>> DeleteRangeAsync(List<TModel> models)
    {
        return Client.DeleteRangeAsync(models);
    }

    /// <summary>
    ///     Retrieves all models.
    /// </summary>
    /// <returns>A unary result containing a list of all models.</returns>
    public virtual async UnaryResult<List<TModel>> ReadAsync()
    {
        var result = await Client.ReadAsync();

        return result;
    }

    /// <summary>
    /// Initiates a server streaming operation to asynchronously retrieve a stream of model data in batches.
    /// </summary>
    /// <param name="batchSize">The number of items to retrieve in each batch.</param>
    /// <returns>A task representing the server streaming result containing a stream of model data.</returns>
    public virtual async Task<ServerStreamingResult<List<TModel>>> StreamReadAllAsync(int batchSize)
    {
        
        
        return await Client.StreamReadAllAsync(batchSize);
    }

    /// <summary>
    ///     Finds models asynchronously based on the provided parameters.
    /// </summary>
    /// <param name="parameters">A byte array containing the parameters to search for the models.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing a list of the found models.</returns>
    public UnaryResult<List<TModel>> FindByParametersAsync(byte[] parameters)
    {
        return Client.FindByParametersAsync(parameters);
    }

   
}
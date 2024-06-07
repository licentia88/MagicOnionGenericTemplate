namespace MagicT.Tests.Services.Interfaces;

public interface IMagicTestService<TModel>  
{
    /// <summary>
    ///     Creates a new instance of the specified model.
    /// </summary>
    /// <param name="model">The model to create.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CreateAsync(TModel model);

    /// <summary>
    ///     Creates multiple instances of the specified model asynchronously.
    /// </summary>
    /// <param name="models">The list of models to create.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task CreateRangeAsync(List<TModel> models);

    /// <summary>
    ///     Updates multiple instances of the specified model asynchronously.
    /// </summary>
    /// <param name="models">The list of models to update.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task UpdateRangeAsync(List<TModel> models);

    /// <summary>
    ///     Deletes multiple instances of the specified model asynchronously.
    /// </summary>
    /// <param name="models">The list of models to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task DeleteRangeAsync(List<TModel> models);

    /// <summary>
    ///     Retrieves all models.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ReadAsync();

    /// <summary>
    ///     Updates the specified model.
    /// </summary>
    /// <param name="model">The model to update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(TModel model);

    /// <summary>
    ///     Deletes the specified model.
    /// </summary>
    /// <param name="model">The model to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(TModel model);

    /// <summary>
    ///  Retrieves a list of models based on given parameters.
    /// </summary>
    /// <param name="parameterBytes">The parameter bytes.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task FindByParametersAsync(byte[] parameterBytes);

  
    /// <summary>
    ///     Retrieves all models with batches.
    /// </summary>
    /// <param name="batchSize">The batch size.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task StreamReadAllAsync(int batchSize);

    
}

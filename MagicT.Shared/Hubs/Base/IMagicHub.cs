using MagicOnion;
using MagicT.Shared.Models.ServiceModels;

namespace MagicT.Shared.Hubs.Base;

/// <summary>
/// Represents a base interface for a MagicOnion hub with CRUD and streaming operations.
/// </summary>
/// <typeparam name="THub">The hub type itself.</typeparam>
/// <typeparam name="TReceiver">The receiver interface for the hub.</typeparam>
/// <typeparam name="TModel">The type of the model used in CRUD operations.</typeparam>
public interface IMagicHub<THub, TReceiver, TModel> : IStreamingHub<THub, TReceiver>
{
    /// <summary>
    /// Connects the client to the hub asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous connection operation.</returns>
    Task ConnectAsync();

    /// <summary>
    /// Creates a new model on the server asynchronously.
    /// </summary>
    /// <param name="model">The model to create.</param>
    /// <returns>A task representing the asynchronous create operation.</returns>
    Task<RESPONSE_RESULT<TModel>> CreateAsync(TModel model);

    /// <summary>
    /// Reads all models from the server asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous read operation.</returns>
    Task<RESPONSE_RESULT<List<TModel>>> ReadAsync();

    /// <summary>
    /// Streams models from the server asynchronously with the specified batch size.
    /// </summary>
    /// <param name="batchSize">The size of each batch for streaming.</param>
    /// <returns>A task representing the asynchronous streaming read operation.</returns>
    Task StreamReadAsync(int batchSize);

    /// <summary>
    /// Updates an existing model on the server asynchronously.
    /// </summary>
    /// <param name="model">The model to update.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    Task<RESPONSE_RESULT<TModel>> UpdateAsync(TModel model);

    /// <summary>
    /// Deletes an existing model on the server asynchronously.
    /// </summary>
    /// <param name="model">The model to delete.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    Task<RESPONSE_RESULT<TModel>> DeleteAsync(TModel model);

    /// <summary>
    /// Notifies the clients when the collection of models changes on the server.
    /// </summary>
    /// <returns>A task representing the asynchronous collection change notification.</returns>
    Task CollectionChanged();
}
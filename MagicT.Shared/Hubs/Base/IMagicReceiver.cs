namespace MagicT.Shared.Hubs.Base;

/// <summary>
/// Represents a receiver contract for a MagicOnion hub with CRUD and streaming operations.
/// </summary>
/// <typeparam name="TModel">The type of the model used in the hub operations.</typeparam>
public interface IMagicReceiver<TModel>
{
    /// <summary>
    /// Called when a new model is created on the server.
    /// </summary>
    /// <param name="model">The created model.</param>
    void OnCreate(TModel model);

    /// <summary>
    /// Called when models are read from the server.
    /// </summary>
    /// <param name="collection">The collection of read models.</param>
    void OnRead(List<TModel> collection);

    /// <summary>
    /// Called when models are streamed from the server.
    /// </summary>
    /// <param name="collection">The collection of streamed models.</param>
    void OnStreamRead(List<TModel> collection);

    /// <summary>
    /// Called when an existing model is updated on the server.
    /// </summary>
    /// <param name="model">The updated model.</param>
    void OnUpdate(TModel model);

    /// <summary>
    /// Called when an existing model is deleted on the server.
    /// </summary>
    /// <param name="model">The deleted model.</param>
    void OnDelete(TModel model);

    /// <summary>
    /// Called when the collection of models changes on the server.
    /// </summary>
    /// <param name="collection">The updated collection of models.</param>
    void OnCollectionChanged(List<TModel> collection);
}
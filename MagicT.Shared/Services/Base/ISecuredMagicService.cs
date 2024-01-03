using MagicOnion;
using MagicT.Shared.Models.ServiceModels;

namespace MagicT.Shared.Services.Base;
/// <summary>
///     Represents a generic service with CRUD operations.
/// </summary>
/// <typeparam name="TService">The type of service.</typeparam>
/// <typeparam name="TModel">The type of model.</typeparam>
public interface ISecureMagicService<TService, TModel> : IMagicService<TService,TModel>
{
    /// <summary>
    /// Creates a new instance of the specified model using encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted data containing the model to create.</param>
    /// <returns>A unary result containing the created model.</returns>
    UnaryResult<EncryptedData<TModel>> CreateEncrypted(EncryptedData<TModel> encryptedData);

    /// <summary>
    /// Retrieves all models using encrypted data.
    /// </summary>
    /// <returns>A unary result containing a list of all models.</returns>
    UnaryResult<EncryptedData<List<TModel>>> ReadEncrypted();

    /// <summary>
    /// Updates the specified model using encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted data containing the model to update.</param>
    /// <returns>A unary result containing the updated model.</returns>
    UnaryResult<EncryptedData<TModel>> UpdateEncrypted(EncryptedData<TModel> encryptedData);

    /// <summary>
    /// Deletes the specified model using encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted data containing the model to delete.</param>
    /// <returns>A unary result containing the deleted model.</returns>
    UnaryResult<EncryptedData<TModel>> DeleteEncrypted(EncryptedData<TModel> encryptedData);


    /// <summary>
    /// Retrieves a list of encrypted data items of a specified model type that are associated with a parent.
    /// </summary>
    /// <param name="parentId">Encrypted identifier of the parent item.</param>
    /// <param name="foreignKey">Encrypted foreign key used for filtering.</param>
    /// <returns>An encrypted response containing a list of <typeparamref name="TModel"/> items.</returns>
    UnaryResult<EncryptedData<List<TModel>>> FindByParentEncrypted(EncryptedData<string> parentId, EncryptedData<string> foreignKey);

    /// <summary>
    ///  Retrieves a list of models based on given parameters
    /// </summary>
    /// <returns>A unary result containing a list of models.</returns>
    UnaryResult<EncryptedData<List<TModel>>> FindByParametersEncrypted(EncryptedData<byte[]> parameterBytes);


    /// <summary>
    /// Streams and reads encrypted data items of a specified model type in batches.
    /// </summary>
    /// <param name="batchSize">Encrypted batch size indicating the number of items to retrieve per batch.</param>
    /// <returns>An encrypted response containing a list of <typeparamref name="TModel"/> items.</returns>
    Task<ServerStreamingResult<EncryptedData<List<TModel>>>> StreamReadAllEncypted(int batchSize);
}


using MagicOnion;
using MagicT.Shared.Models.ServiceModels;

namespace MagicT.Client.Services.Base;

/// <summary>
/// Helper 
/// </summary>
/// <typeparam name="TModel"></typeparam>
public interface ISecureClientMethods<TModel>
{
    /// <summary>
    /// Creates a new instance of the specified model using encrypted data.
    /// </summary>
    /// <returns>A unary result containing the created model.</returns>
    UnaryResult<TModel> CreateEncryptedAsync(TModel model);

    /// <summary>
    ///     Creates multiple instances of the specified model asynchronously and returns the result as encrypted data.
    /// </summary>
    /// <param name="models">The list of models to create.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the created models.</returns>
    UnaryResult<List<TModel>> CreateRangeEncryptedAsync(List<TModel> models);

    /// <summary>
    /// Retrieves all models using encrypted data.
    /// </summary>
    /// <returns>A unary result containing a list of all models.</returns>
    UnaryResult<List<TModel>> ReadEncryptedAsync();

    /// <summary>
    /// Updates the specified model using encrypted data.
    /// </summary>
    /// <returns>A unary result containing the updated model.</returns>
    UnaryResult<TModel> UpdateEncryptedAsync(TModel model);

    /// <summary>
    ///     Updates multiple instances of the specified model asynchronously with encryption.
    /// </summary>
    /// <param name="models">The list of models to update.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the updated models.</returns>
    UnaryResult<List<TModel>> UpdateRangeEncryptedAsync(List<TModel> models);

    /// <summary>
    ///     Deletes the specified model asynchronously with encryption.
    /// </summary>
    /// <param name="model">The model to delete.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the deleted model.</returns>
    UnaryResult<TModel> DeleteEncryptedAsync(TModel model);

    /// <summary>
    ///     Deletes multiple instances of the specified model asynchronously with encryption.
    /// </summary>
    /// <param name="models">The list of models to delete.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> indicating the success of the deletion operation.</returns>
    UnaryResult<List<TModel>> DeleteRangeEncryptedAsync(List<TModel> models);

    /// <summary>
    ///     Finds models by parent identifiers asynchronously with encryption.
    /// </summary>
    /// <param name="parentId">The encrypted parent identifier.</param>
    /// <param name="foreignKey">The encrypted foreign key.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted list of models.</returns>
    UnaryResult<List<TModel>> FindByParentEncryptedAsync(string parentId, string foreignKey);

    /// <summary>
    ///     Finds models by parameters asynchronously with encryption.
    /// </summary>
    /// <param name="parameterBytes">The encrypted parameters to search for the models.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted list of models.</returns>
    UnaryResult<List<TModel>> FindByParametersEncryptedAsync(byte[] parameterBytes);

    /// <summary>
    ///     Streams and reads all models asynchronously with encryption.
    /// </summary>
    /// <param name="batchSize">The size of each batch.</param>
    /// <returns>An asynchronous stream of lists of encrypted models.</returns>
    IAsyncEnumerable<List<TModel>> StreamReadAllEncryptedAsync(int batchSize);

}


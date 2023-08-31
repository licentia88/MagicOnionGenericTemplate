using MagicOnion;

namespace MagicT.Shared.Services.Base;

/// <summary>
///     Represents a generic service with CRUD operations.
/// </summary>
/// <typeparam name="TService">The type of service.</typeparam>
/// <typeparam name="TModel">The type of model.</typeparam>
public interface ISecuredMagicService<TService, TModel> : IMagicService<TService,TModel>
{    
    /// <summary>
    /// Creates a new instance of the specified model using encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted data containing the model to create.</param>
    /// <returns>A unary result containing the created model.</returns>
    UnaryResult<TModel> CreateEncrypted(TModel model);

    /// <summary>
    /// Retrieves all models using encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted data containing the request parameters.</param>
    /// <returns>A unary result containing a list of all models.</returns>
    new UnaryResult<List<TModel>> ReadEncrypted();

    /// <summary>
    /// Updates the specified model using encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted data containing the model to update.</param>
    /// <returns>A unary result containing the updated model.</returns>
    UnaryResult<TModel> UpdateEncrypted(TModel model);

    /// <summary>
    /// Deletes the specified model using encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted data containing the model to delete.</param>
    /// <returns>A unary result containing the deleted model.</returns>
    UnaryResult<TModel> DeleteEncrypted(TModel model);


    UnaryResult<List<TModel>> FindByParentEncrypted(string parentId, string foreignKey);


    new IAsyncEnumerable<List<TModel>> StreamReadAllEncypted(int bathcSize);
}


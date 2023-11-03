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
    /// <returns>A unary result containing the created model.</returns>
    UnaryResult<TModel> CreateEncryptedAsync(TModel model);

    /// <summary>
    /// Retrieves all models using encrypted data.
    /// </summary>
    /// <returns>A unary result containing a list of all models.</returns>
    new UnaryResult<List<TModel>> ReadEncryptedAsync();

    /// <summary>
    /// Updates the specified model using encrypted data.
    /// </summary>
    /// <returns>A unary result containing the updated model.</returns>
    UnaryResult<TModel> UpdateEncryptedAsync(TModel model);

    /// <summary>
    /// Deletes the specified model using encrypted data.
    /// </summary>
    /// <returns>A unary result containing the deleted model.</returns>
    UnaryResult<TModel> DeleteEncryptedAsync(TModel model);


    UnaryResult<List<TModel>> FindByParentEncryptedAsync(string parentId, string foreignKey);

    UnaryResult<List<TModel>> FindByParametersEncryptedAsyncAsync(byte[] parameterBytes);

    new IAsyncEnumerable<List<TModel>> StreamReadAllEncyptedAsync(int bathcSize);
}


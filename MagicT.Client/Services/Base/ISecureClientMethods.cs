using MagicOnion;

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
    /// Deletes the specified model using encrypted data.
    /// </summary>
    /// <returns>A unary result containing the deleted model.</returns>
    UnaryResult<TModel> DeleteEncryptedAsync(TModel model);


    UnaryResult<List<TModel>> FindByParentEncryptedAsync(string parentId, string foreignKey);

    UnaryResult<List<TModel>> FindByParametersEncryptedAsync(byte[] parameterBytes);

    IAsyncEnumerable<List<TModel>> StreamReadAllEncyptedAsync(int bathcSize);
}


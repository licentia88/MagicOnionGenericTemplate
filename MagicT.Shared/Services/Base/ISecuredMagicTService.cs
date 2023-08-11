using MagicOnion;
using MagicT.Shared.Models.ServiceModels;

namespace MagicT.Shared.Services.Base;

/// <summary>
///     Represents a generic service with CRUD operations.
/// </summary>
/// <typeparam name="TService">The type of service.</typeparam>
/// <typeparam name="TModel">The type of model.</typeparam>
public interface ISecuredMagicTService<TService, TModel> : IMagicTService<TService,TModel>
{    
    /// <summary>
    /// Creates a new instance of the specified model using encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted data containing the model to create.</param>
    /// <returns>A unary result containing the created model.</returns>
    new UnaryResult<TModel> CreateEncrypted(EncryptedData<TModel> encryptedData);

    /// <summary>
    /// Retrieves all models using encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted data containing the request parameters.</param>
    /// <returns>A unary result containing a list of all models.</returns>
    new UnaryResult<List<TModel>> ReadAllEncrypted();

    /// <summary>
    /// Updates the specified model using encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted data containing the model to update.</param>
    /// <returns>A unary result containing the updated model.</returns>
    new UnaryResult<TModel> UpdateEncrypted(EncryptedData<TModel> encryptedData);

    /// <summary>
    /// Deletes the specified model using encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted data containing the model to delete.</param>
    /// <returns>A unary result containing the deleted model.</returns>
    new UnaryResult<TModel> DeleteEncrypted(EncryptedData<TModel> encryptedData);
}


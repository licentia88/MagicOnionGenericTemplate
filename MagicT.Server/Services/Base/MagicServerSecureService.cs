using Benutomo;
using MagicOnion;
using MagicT.Server.Filters;
using MagicT.Server.Helpers;
using MagicT.Shared.Cryptography;
using MagicT.Shared.Extensions;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services.Base;
using Mapster;

namespace MagicT.Server.Services.Base;

/// <summary>
/// A secure service class that provides encrypted database operations and auditing functionality.
/// </summary>
/// <typeparam name="TService">The type of the service.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TContext">The type of the database context.</typeparam>
[Authorize]
[AutomaticDisposeImpl]
public abstract partial class MagicServerSecureService<TService, TModel, TContext> : AuditDatabaseService<TService, TModel, TContext>, IMagicSecureService<TService, TModel>
    where TService : IMagicSecureService<TService, TModel>, IService<TService>
    where TModel : class
    where TContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MagicServerSecureService{TService,TModel,TContext}"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    protected MagicServerSecureService(IServiceProvider provider) : base(provider)
    {
    }
    
    ~MagicServerSecureService()
    {
        Dispose(false);
    }

    /// <summary>
    /// Creates a new instance of the specified model asynchronously with encryption.
    /// </summary>
    /// <param name="encryptedData">The encrypted data of the model to create.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted created model.</returns>
    public virtual async UnaryResult<EncryptedData<TModel>> CreateEncrypted(EncryptedData<TModel> encryptedData)
    {
        // Decrypt the encrypted data.
        var decryptedData = CryptoHelper.DecryptData(encryptedData, SharedKey);

        // Create the decrypted model asynchronously.
        var response = await CreateAsync(decryptedData);

        // Encrypt the response model.
        var cryptedData = CryptoHelper.EncryptData(response, SharedKey);

        return cryptedData;
    }

    /// <summary>
    /// Creates multiple instances of the specified model asynchronously with encryption.
    /// </summary>
    /// <param name="encryptedData">The encrypted data of the models to create.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted created models.</returns>
    public async UnaryResult<EncryptedData<List<TModel>>> CreateRangeEncryptedAsync(EncryptedData<List<TModel>> encryptedData)
    {
        // Decrypt the encrypted data.
        var decryptedData = CryptoHelper.DecryptData(encryptedData, SharedKey);

        // Create the decrypted models asynchronously.
        var response = await CreateRangeAsync(decryptedData);

        // Encrypt the response models.
        var cryptedData = CryptoHelper.EncryptData(response, SharedKey);

        return cryptedData;
    }

    /// <summary>
    /// Reads all models asynchronously with encryption.
    /// </summary>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted list of models.</returns>
    public virtual async UnaryResult<EncryptedData<List<TModel>>> ReadEncrypted()
    {
        // Get the shared key.
        var sharedKey = SharedKey;

        // Read the models asynchronously.
        var response = await ReadAsync();

        // Encrypt the response models.
        return CryptoHelper.EncryptData(response, sharedKey);
    }

    /// <summary>
    /// Updates an existing model asynchronously with encryption.
    /// </summary>
    /// <param name="encryptedData">The encrypted data of the model to update.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted updated model.</returns>
    public virtual async UnaryResult<EncryptedData<TModel>> UpdateEncrypted(EncryptedData<TModel> encryptedData)
    {
        // Decrypt the encrypted data.
        var decryptedData = CryptoHelper.DecryptData(encryptedData, SharedKey);

        // Update the decrypted model asynchronously.
        var response = await UpdateAsync(decryptedData);

        // Encrypt the response model.
        return CryptoHelper.EncryptData(response, SharedKey);
    }

    /// <summary>
    /// Updates multiple instances of the specified model asynchronously with encryption.
    /// </summary>
    /// <param name="encryptedData">The encrypted data of the models to update.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted updated models.</returns>
    public async UnaryResult<EncryptedData<List<TModel>>> UpdateRangeEncrypted(EncryptedData<List<TModel>> encryptedData)
    {
        // Decrypt the encrypted data.
        var decryptedData = CryptoHelper.DecryptData(encryptedData, SharedKey);

        // Update the decrypted models asynchronously.
        var response = await UpdateRangeAsync(decryptedData);

        // Encrypt the response models.
        return CryptoHelper.EncryptData(response, SharedKey);
    }

    /// <summary>
    /// Deletes an existing model asynchronously with encryption.
    /// </summary>
    /// <param name="encryptedData">The encrypted data of the model to delete.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted deleted model.</returns>
    public virtual async UnaryResult<EncryptedData<TModel>> DeleteEncrypted(EncryptedData<TModel> encryptedData)
    {
        // Decrypt the encrypted data.
        var decryptedData = CryptoHelper.DecryptData(encryptedData, SharedKey);

        // Delete the decrypted model asynchronously.
        var response = await DeleteAsync(decryptedData);

        // Encrypt the response model.
        return CryptoHelper.EncryptData(response, SharedKey);
    }

    /// <summary>
    /// Deletes multiple instances of the specified model asynchronously with encryption.
    /// </summary>
    /// <param name="encryptedData">The encrypted data of the models to delete.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> indicating the success of the deletion operation.</returns>
    public async UnaryResult<EncryptedData<List<TModel>>> DeleteRangeEncrypted(EncryptedData<List<TModel>> encryptedData)
    {
        // Decrypt the encrypted data.
        var decryptedData = CryptoHelper.DecryptData(encryptedData, SharedKey);

        // Delete the decrypted models asynchronously.
        var response = await DeleteRangeAsync(decryptedData);

        // Encrypt the response models.
        return CryptoHelper.EncryptData(response, SharedKey);
    }

    /// <summary>
    /// Finds models by parent identifiers asynchronously with encryption.
    /// </summary>
    /// <param name="parentId">The encrypted parent identifier.</param>
    /// <param name="foreignKey">The encrypted foreign key.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted list of models.</returns>
    public virtual async UnaryResult<EncryptedData<List<TModel>>> FindByParentEncrypted(EncryptedData<string> parentId, EncryptedData<string> foreignKey)
    {
        // Decrypt the encrypted parent identifier and foreign key.
        var decryptedKey = CryptoHelper.DecryptData(foreignKey, SharedKey);
        var decryptedId = CryptoHelper.DecryptData(parentId, SharedKey);

        // Find the models by decrypted parent identifiers asynchronously.
        var responseData = await FindByParentAsync(decryptedId, decryptedKey);

        // Encrypt the response models.
        return CryptoHelper.EncryptData(responseData, SharedKey);
    }

    /// <summary>
    /// Finds models by parameters asynchronously with encryption.
    /// </summary>
    /// <param name="parameterBytes">The encrypted parameters to search for the models.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted list of models.</returns>
    public virtual UnaryResult<EncryptedData<List<TModel>>> FindByParametersEncrypted(EncryptedData<byte[]> parameterBytes)
    {
        return ExecuteAsync(async () =>
        {
            // Decrypt the encrypted parameter bytes.
            var decryptedBytes = CryptoHelper.DecryptData(parameterBytes, SharedKey);

            var loParameters = decryptedBytes.DeserializeFromBytes<KeyValuePair<string, Object>[]>();
            // Build the query data for the models.
            var queryData = Db.BuildQuery<TModel>(loParameters);

            // Query the models asynchronously.
            var result = await Db.Manager().QueryAsync(queryData.query, queryData.parameters);

            // Encrypt the response models.
            return CryptoHelper.EncryptData(result.Adapt<List<TModel>>(), SharedKey);
        });
    }

    /// <summary>
    /// Streams and reads all models asynchronously with encryption.
    /// </summary>
    /// <param name="batchSize">The size of each batch.</param>
    /// <returns>A <see cref="ServerStreamingResult{T}"/> containing the encrypted list of models.</returns>
    public virtual async Task<ServerStreamingResult<EncryptedData<List<TModel>>>> StreamReadAllEncrypted(int batchSize)
    {
        // Get the server streaming context for the list of TModel.
        var stream = GetServerStreamingContext<EncryptedData<List<TModel>>>();

        // Iterate through the asynchronously fetched data in batches.
        await foreach (var data in FetchStreamAsync(batchSize))
        {
            // Encrypt the fetched data.
            var responseData = CryptoHelper.EncryptData(data, SharedKey);
            await stream.WriteAsync(responseData);
        }

        // Return the result of the streaming context.
        return stream.Result();
    }
}
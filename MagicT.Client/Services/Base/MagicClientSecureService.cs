using Benutomo;
using Grpc.Core;
using MagicOnion;
using MagicT.Client.Extensions;
using MagicT.Client.Filters;
using MagicT.Shared.Cryptography;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services.Base;

namespace MagicT.Client.Services.Base;


/// <summary>
/// Base class for secure service operations that involve encryption and decryption of data.
/// </summary>
/// <typeparam name="TService">The type of the service.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
[AutomaticDisposeImpl]
public abstract partial class MagicClientSecureService<TService, TModel> : MagicClientService<TService, TModel>, IMagicSecureService<TService, TModel>, ISecureClientMethods<TModel> where TService : IMagicSecureService<TService, TModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MagicClientSecureService{TService,TModel}"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    protected MagicClientSecureService(IServiceProvider provider) : base(provider, new AuthorizationFilter(provider))
    {

    }

    ~MagicClientSecureService()
    {
        Dispose(false);
    }
    
    /// <summary>
    ///     Creates a new instance of the specified model asynchronously with encryption.
    /// </summary>
    /// <param name="model">The model to create.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the created model.</returns>
    public async UnaryResult<TModel> CreateEncryptedAsync(TModel model)
    {
        var sharedKey = await LocalStorageService.GetItemAsync<byte[]>("shared-bin");

        var encryptedData = CryptoHelper.EncryptData(model, sharedKey);

        var result = await Client.CreateEncrypted(encryptedData);

        return CryptoHelper.DecryptData(result, sharedKey);
    }

    /// <summary>
    ///     Creates multiple instances of the specified model asynchronously and returns the result as encrypted data.
    /// </summary>
    /// <param name="models">The list of models to create.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the created models.</returns>
    public async UnaryResult<List<TModel>> CreateRangeEncryptedAsync(List<TModel> models)
    {
        var sharedKey = await LocalStorageService.GetItemAsync<byte[]>("shared-bin");

        var encryptedData = CryptoHelper.EncryptData(models, sharedKey);

        var result = await Client.CreateRangeEncryptedAsync(encryptedData);

        return CryptoHelper.DecryptData(result, sharedKey);
    }
    /// <summary>
    ///     Reads all models asynchronously and decrypts the result.
    /// </summary>
    /// <returns>A <see cref="UnaryResult{T}"/> containing a list of decrypted models.</returns>
    public async UnaryResult<List<TModel>> ReadEncryptedAsync()
    {
        var result = await Client.ReadEncrypted();
        var sharedKey = await LocalStorageService.GetItemAsync<byte[]>("shared-bin");
        return CryptoHelper.DecryptData(result, sharedKey);
    }

    /// <summary>
    ///     Updates an existing model asynchronously with encryption.
    /// </summary>
    /// <param name="model">The model to update.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the updated model.</returns>
    public async UnaryResult<TModel> UpdateEncryptedAsync(TModel model)
    {
        var sharedKey = await LocalStorageService.GetItemAsync<byte[]>("shared-bin");

        var encryptedData = CryptoHelper.EncryptData(model, sharedKey);

        var result = await Client.UpdateEncrypted(encryptedData);

        return CryptoHelper.DecryptData(result, sharedKey);
    }

    /// <summary>
    ///     Updates multiple instances of the specified model asynchronously with encryption.
    /// </summary>
    /// <param name="models">The list of models to update.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the updated models.</returns>
    public async UnaryResult<List<TModel>> UpdateRangeEncryptedAsync(List<TModel> models)
    {
        var sharedKey = await LocalStorageService.GetItemAsync<byte[]>("shared-bin");

        var encryptedData = CryptoHelper.EncryptData(models, sharedKey);

        var result = await Client.UpdateRangeEncrypted(encryptedData);

        return CryptoHelper.DecryptData(result, sharedKey);
    }

    


    /// <summary>
    ///     Deletes an existing model asynchronously with encryption.
    /// </summary>
    /// <param name="model">The model to delete.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the deleted model.</returns>
    public async UnaryResult<TModel> DeleteEncryptedAsync(TModel model)
    {
        var sharedKey = await LocalStorageService.GetItemAsync<byte[]>("shared-bin");

        var encryptedData = CryptoHelper.EncryptData(model, sharedKey);

        var result = await Client.DeleteEncrypted(encryptedData);

        return CryptoHelper.DecryptData(result, sharedKey);
    }

    /// <summary>
    ///     Deletes multiple instances of the specified model asynchronously with encryption.
    /// </summary>
    /// <param name="models">The list of models to delete.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> indicating the success of the deletion operation.</returns>
    public async UnaryResult<List<TModel>> DeleteRangeEncryptedAsync(List<TModel> models)
    {
        var sharedKey = await LocalStorageService.GetItemAsync<byte[]>("shared-bin");

        var encryptedData = CryptoHelper.EncryptData(models, sharedKey);

        var result = await Client.DeleteRangeEncrypted(encryptedData);

        return CryptoHelper.DecryptData(result, sharedKey);
    }
    /// <summary>
    ///     Finds models by parent identifiers asynchronously with encryption.
    /// </summary>
    /// <param name="parentId">The parent identifier.</param>
    /// <param name="foreignKey">The foreign key.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing a list of decrypted models.</returns>
    public async UnaryResult<List<TModel>> FindByParentEncryptedAsync(string parentId, string foreignKey)
    {
        var sharedKey = await LocalStorageService.GetItemAsync<byte[]>("shared-bin");

        var encryptedParentId = CryptoHelper.EncryptData(parentId, sharedKey);

        var encryptedForeignKey = CryptoHelper.EncryptData(foreignKey, sharedKey);

        var result = await Client.FindByParentEncrypted(encryptedParentId, encryptedForeignKey);

        return CryptoHelper.DecryptData(result, sharedKey);
    }


    /// <summary>
    ///     Streams and reads all models asynchronously with encryption.
    /// </summary>
    /// <param name="batchSize">The size of each batch.</param>
    /// <returns>An asynchronous stream of lists of decrypted models.</returns>
    public async IAsyncEnumerable<List<TModel>> StreamReadAllEncryptedAsync(int batchSize)
    {
        var sharedKey = await LocalStorageService.GetItemAsync<byte[]>("shared-bin");

        var result = await Client.StreamReadAllEncrypted(batchSize);

        await foreach (var responseData in result.ResponseStream.ReadAllAsync())
        {
            var decrypted = CryptoHelper.DecryptData(responseData, sharedKey);

            yield return decrypted;
        }
    }


    /// <summary>
    ///     Finds models by parameters asynchronously with encryption.
    /// </summary>
    /// <param name="parameterBytes">A byte array containing the parameters to search for the models.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing a list of decrypted models.</returns>
    public async UnaryResult<List<TModel>> FindByParametersEncryptedAsync(byte[] parameterBytes)
    {
        var sharedKey = await LocalStorageService.GetItemAsync<byte[]>("shared-bin");

        var encryptedData = CryptoHelper.EncryptData(parameterBytes, sharedKey);

        var result = await Client.FindByParametersEncrypted(encryptedData);

        var decryptedData = CryptoHelper.DecryptData(result, sharedKey);

        return decryptedData;
    }

    /// <inheritdoc/>
    public override async Task<ServerStreamingResult<List<TModel>>> StreamReadAllAsync(int batchSize)
    {
        var authFilter = Filters.Get<AuthorizationFilter>();
        var header = await authFilter.CreateHeaderAsync();

        var metaData = new Metadata();

        metaData.AddOrUpdateItem(header.Key, header.Data);

        return await base.WithHeaders(metaData).StreamReadAllAsync(batchSize);
    }

    /// <summary>
    ///     Creates a new instance of the specified model with encryption.
    /// </summary>
    /// <param name="encryptedData">The encrypted data of the model to create.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted created model.</returns>
    public UnaryResult<EncryptedData<TModel>> CreateEncrypted(EncryptedData<TModel> encryptedData)
    {
        return Client.CreateEncrypted(encryptedData);
    }

    /// <summary>
    ///     Reads all models with encryption.
    /// </summary>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted list of models.</returns>
    public UnaryResult<EncryptedData<List<TModel>>> ReadEncrypted()
    {
        return Client.ReadEncrypted();
    }

    /// <summary>
    ///     Updates an existing model with encryption.
    /// </summary>
    /// <param name="encryptedData">The encrypted data of the model to update.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted updated model.</returns>
    public UnaryResult<EncryptedData<TModel>> UpdateEncrypted(EncryptedData<TModel> encryptedData)
    {
        return Client.UpdateEncrypted(encryptedData);
    }

    /// <summary>
    ///     Deletes an existing model with encryption.
    /// </summary>
    /// <param name="encryptedData">The encrypted data of the model to delete.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted deleted model.</returns>
    public UnaryResult<EncryptedData<TModel>> DeleteEncrypted(EncryptedData<TModel> encryptedData)
    {
        return Client.DeleteEncrypted(encryptedData);
    }

    /// <summary>
    ///     Finds models by parent identifiers with encryption.
    /// </summary>
    /// <param name="parentId">The encrypted parent identifier.</param>
    /// <param name="foreignKey">The encrypted foreign key.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted list of models.</returns>
    public UnaryResult<EncryptedData<List<TModel>>> FindByParentEncrypted(EncryptedData<string> parentId, EncryptedData<string> foreignKey)
    {
        return Client.FindByParentEncrypted(parentId, foreignKey);
    }

    /// <summary>
    ///     Streams and reads all models with encryption.
    /// </summary>
    /// <param name="batchSize">The size of each batch.</param>
    /// <returns>A <see cref="ServerStreamingResult{T}"/> containing the encrypted list of models.</returns>
    public Task<ServerStreamingResult<EncryptedData<List<TModel>>>> StreamReadAllEncrypted(int batchSize)
    {
        return Client.StreamReadAllEncrypted(batchSize);
    }

    /// <summary>
    ///     Finds models by parameters with encryption.
    /// </summary>
    /// <param name="parameterBytes">The encrypted parameters to search for the models.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted list of models.</returns>
    public UnaryResult<EncryptedData<List<TModel>>> FindByParametersEncrypted(EncryptedData<byte[]> parameterBytes)
    {
        return Client.FindByParametersEncrypted(parameterBytes);
    }

    /// <summary>
    ///     Creates multiple instances of the specified model asynchronously and returns the result as encrypted data.
    /// </summary>
    /// <param name="models">The encrypted data containing the list of models to create.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the created models wrapped in an <see cref="EncryptedData{T}"/> object.</returns>
    public UnaryResult<EncryptedData<List<TModel>>> CreateRangeEncryptedAsync(EncryptedData<List<TModel>> models)
    {
        return Client.CreateRangeEncryptedAsync(models);

    }

    /// <summary>
    ///     Updates multiple instances of the specified model asynchronously with encryption.
    /// </summary>
    /// <param name="encryptedData">The encrypted data of the models to update.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted updated models.</returns>
    public UnaryResult<EncryptedData<List<TModel>>> UpdateRangeEncrypted(EncryptedData<List<TModel>> encryptedData)
    {
        return Client.UpdateRangeEncrypted(encryptedData);
    }

    /// <summary>
    ///     Deletes multiple instances of the specified model asynchronously with encryption.
    /// </summary>
    /// <param name="encryptedData">The encrypted data of the models to delete.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> indicating the success of the deletion operation.</returns>
    public UnaryResult<EncryptedData<List<TModel>>> DeleteRangeEncrypted(EncryptedData<List<TModel>> encryptedData)
    {
        return Client.DeleteRangeEncrypted(encryptedData);

    }

   
}

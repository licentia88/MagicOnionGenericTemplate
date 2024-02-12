using Grpc.Core;
using MagicOnion;
using MagicT.Client.Extensions;
using MagicT.Client.Filters;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services.Base;

namespace MagicT.Client.Services.Base;


/// <summary>
/// Base class for secure service operations that involve encryption and decryption of data.
/// </summary>
/// <typeparam name="TService">The type of the service.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
public abstract class MagicClientSecureService<TService, TModel> : MagicClientService<TService, TModel>, ISecureMagicService<TService, TModel>, ISecureClientMethods<TModel>
    where TService : ISecureMagicService<TService, TModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MagicClientSecureService{TService,TModel}"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    protected MagicClientSecureService(IServiceProvider provider) : base(provider, new AuthorizationFilter(provider))
    {

    }


    /// <inheritdoc/>
    public async UnaryResult<TModel> CreateEncryptedAsync(TModel model)
    {
        var sharedKey = await LocalStorageService.GetItemAsync<byte[]>("shared-bin");

        var encryptedData = CryptoHelper.EncryptData(model,sharedKey);

        var result = await Client.CreateEncrypted(encryptedData);
        
        return CryptoHelper.DecryptData(result, sharedKey);
    }

    /// <inheritdoc/>
    public async UnaryResult<List<TModel>> ReadEncryptedAsync()
    {
        var result = await Client.ReadEncrypted();
        var sharedKey = await LocalStorageService.GetItemAsync<byte[]>("shared-bin");
        return CryptoHelper.DecryptData(result, sharedKey);
    }

    /// <inheritdoc/>
    public  async UnaryResult<TModel> UpdateEncryptedAsync(TModel model)
    {
        var sharedKey = await LocalStorageService.GetItemAsync<byte[]>("shared-bin");

        var encryptedData = CryptoHelper.EncryptData(model, sharedKey);

        var result = await Client.UpdateEncrypted(encryptedData);

        return CryptoHelper.DecryptData(result, sharedKey);
    }

    /// <inheritdoc/>
    public  async UnaryResult<TModel> DeleteEncryptedAsync(TModel model)
    {
        var sharedKey = await LocalStorageService.GetItemAsync<byte[]>("shared-bin");

        var encryptedData = CryptoHelper.EncryptData(model, sharedKey);

        var result = await Client.DeleteEncrypted(encryptedData);

        return  CryptoHelper.DecryptData(result, sharedKey);
    }

    public async UnaryResult<List<TModel>> FindByParentEncryptedAsync(string parentId, string foreignKey)
    {
        var sharedKey = await LocalStorageService.GetItemAsync<byte[]>("shared-bin");

        var encryptedParentId =  CryptoHelper.EncryptData(parentId, sharedKey);

        var encryptedForeignKey =  CryptoHelper.EncryptData(foreignKey, sharedKey);

        var result = await Client.FindByParentEncrypted(encryptedParentId,encryptedForeignKey);

        return  CryptoHelper.DecryptData(result, sharedKey);
    }

    public async IAsyncEnumerable<List<TModel>> StreamReadAllEncyptedAsync(int  bathcSize)
    {
        var sharedKey = await LocalStorageService.GetItemAsync<byte[]>("shared-bin");
 
        var result = await Client.StreamReadAllEncypted(bathcSize);

        await foreach( var responseData in result.ResponseStream.ReadAllAsync())
        {
            var decrypted = CryptoHelper.DecryptData(responseData, sharedKey);

            yield return decrypted;
        }     
    }

    public async UnaryResult<List<TModel>> FindByParametersEncryptedAsync(byte[] parameterBytes)
    {
        var sharedKey = await LocalStorageService.GetItemAsync<byte[]>("shared-bin");

        var encryptedData = CryptoHelper.EncryptData(parameterBytes, sharedKey);

        var result = await Client.FindByParametersEncrypted(encryptedData);

        var decryptedData = CryptoHelper.DecryptData(result, sharedKey);

        return decryptedData;
    }

    public override async Task<ServerStreamingResult<List<TModel>>> StreamReadAllAsync(int batchSize)
    {
        var authFilter = Filters.Get<AuthorizationFilter>();
        var header = await authFilter.CreateHeaderAsync();

        var metaData = new Metadata();

        metaData.AddOrUpdateItem(header.Key, header.Data);

        return await base.WithHeaders(metaData).StreamReadAllAsync(batchSize);
    }

    public UnaryResult<EncryptedData<TModel>> CreateEncrypted(EncryptedData<TModel> encryptedData)
    {
        return Client.CreateEncrypted(encryptedData);
    }

    public UnaryResult<EncryptedData<List<TModel>>> ReadEncrypted()
    {
        return Client.ReadEncrypted();
    }

    public UnaryResult<EncryptedData<TModel>> UpdateEncrypted(EncryptedData<TModel> encryptedData)
    {
        return Client.UpdateEncrypted(encryptedData);
    }

    public UnaryResult<EncryptedData<TModel>> DeleteEncrypted(EncryptedData<TModel> encryptedData)
    {
        return Client.DeleteEncrypted(encryptedData);
    }

    public UnaryResult<EncryptedData<List<TModel>>> FindByParentEncrypted(EncryptedData<string> parentId, EncryptedData<string> foreignKey)
    {
        return Client.FindByParentEncrypted(parentId,foreignKey);
    }

    public Task<ServerStreamingResult<EncryptedData<List<TModel>>>> StreamReadAllEncypted(int batchSize)
    {
        return Client.StreamReadAllEncypted(batchSize);
    }

    public UnaryResult<EncryptedData<List<TModel>>> FindByParametersEncrypted(EncryptedData<byte[]> parameterBytes)
    {
        return Client.FindByParametersEncrypted(parameterBytes);
    }
}

using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;
using MagicT.Shared.Helpers;
using MagicT.Shared.Services.Base;
using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Services.Base;


/// <summary>
/// Base class for secure service operations that involve encryption and decryption of data.
/// </summary>
/// <typeparam name="TService">The type of the service.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
public abstract class MagicClientSecureServiceBase<TService, TModel> : MagicClientServiceBase<TService, TModel>, ISecuredMagicService<TService, TModel>
    where TService : IMagicService<TService, TModel>
{

    /// <summary>
    /// Gets or sets the local storage service for managing client-side storage.
    /// </summary>
    private ILocalStorageService Storage { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MagicClientSecureServiceBase{TService,TModel}"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    /// <param name="filters">An array of client filters.</param>
    protected MagicClientSecureServiceBase(IServiceProvider provider, params IClientFilter[] filters) : base(provider, filters)
    {
        Storage = provider.GetService<ILocalStorageService>();
    }


    /// <inheritdoc/>
    public  async UnaryResult<TModel> CreateEncryptedAsync(TModel model)
    {
        var sharedKey = await Storage.GetItemAsync<byte[]>("shared-bin");

        var encryptedData = CryptoHelper.EncryptData(model,sharedKey);

        var result = await Client.CreateEncrypted(encryptedData);
        
        return CryptoHelper.DecryptData(result, sharedKey);
    }

    /// <inheritdoc/>
    public async UnaryResult<List<TModel>> ReadEncryptedAsync()
    {
        var result = await Client.ReadEncryptedAsync();
        var sharedKey = await Storage.GetItemAsync<byte[]>("shared-bin");
        return CryptoHelper.DecryptData(result, sharedKey);
    }

    /// <inheritdoc/>
    public  async UnaryResult<TModel> UpdateEncryptedAsync(TModel model)
    {
        var sharedKey = await Storage.GetItemAsync<byte[]>("shared-bin");

        var encryptedData = CryptoHelper.EncryptData(model, sharedKey);

        var result = await Client.UpdateEncrypted(encryptedData);

        return CryptoHelper.DecryptData(result, sharedKey);
    }

    /// <inheritdoc/>
    public  async UnaryResult<TModel> DeleteEncryptedAsync(TModel model)
    {
        var sharedKey = await Storage.GetItemAsync<byte[]>("shared-bin");

        var encryptedData = CryptoHelper.EncryptData(model, sharedKey);

        var result = await Client.DeleteEncryptedAsync(encryptedData);

        return  CryptoHelper.DecryptData(result, sharedKey);
    }


    public async UnaryResult<List<TModel>> FindByParentEncryptedAsync(string parentId, string foreignKey)
    {
        var sharedKey = await Storage.GetItemAsync<byte[]>("shared-bin");

        var encryptedParentId =  CryptoHelper.EncryptData(parentId, sharedKey);

        var encryptedForeignKey =  CryptoHelper.EncryptData(foreignKey, sharedKey);

        var result = await Client.FindByParentEncryptedAsync(encryptedParentId,encryptedForeignKey);

        return  CryptoHelper.DecryptData(result, sharedKey);
    }

    public async IAsyncEnumerable<List<TModel>> StreamReadAllEncyptedAsync(int  bathcSize)
    {
        var sharedKey = await Storage.GetItemAsync<byte[]>("shared-bin");
 
        var result = await Client.StreamReadAllEncyptedAsync(bathcSize);

        await foreach( var responseData in result.ResponseStream.ReadAllAsync())
        {
            var decrypted = CryptoHelper.DecryptData(responseData, sharedKey);

            yield return decrypted;
        }     
    }

    public async UnaryResult<List<TModel>> FindByParametersEncryptedAsyncAsync(byte[] parameterBytes)
    {
        var sharedKey = await Storage.GetItemAsync<byte[]>("shared-bin");

        var encryptedData = CryptoHelper.EncryptData(parameterBytes, sharedKey);

        var result = await Client.FindByParametersEncryptedAsync(encryptedData);

        var decryptedData = CryptoHelper.DecryptData(result, sharedKey);

        return decryptedData;
    }
}

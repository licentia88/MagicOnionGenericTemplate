using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;
using MagicT.Shared.Helpers;
using MagicT.Shared.Services.Base;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Services.Base;


/// <summary>
/// Base class for secure service operations that involve encryption and decryption of data.
/// </summary>
/// <typeparam name="TService">The type of the service.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
public abstract class MagicTClientSecureServiceBase<TService, TModel> : MagicTClientServiceBase<TService, TModel>, ISecuredMagicTService<TService, TModel>
    where TService : IMagicTService<TService, TModel>
{

    /// <summary>
    /// Gets or sets the local storage service for managing client-side storage.
    /// </summary>
    private ILocalStorageService Storage { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MagicTClientSecureServiceBase{TService,TModel}"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    /// <param name="filters">An array of client filters.</param>
    protected MagicTClientSecureServiceBase(IServiceProvider provider, params IClientFilter[] filters) : base(provider, filters)
    {
        Storage = provider.GetService<ILocalStorageService>();
    }


    /// <inheritdoc/>
    public  async UnaryResult<TModel> CreateEncrypted(TModel model)
    {
        var sharedKey = await Storage.GetItemAsync<byte[]>("shared-bin");

        var encryptedData = CryptoHelper.EncryptData(model,sharedKey);

        var result = await Client.CreateEncrypted(encryptedData);
        
        return CryptoHelper.DecryptData(result, sharedKey);
    }

    /// <inheritdoc/>
    public async UnaryResult<List<TModel>> ReadAllEncrypted()
    {
        var result = await Client.ReadAllEncrypted();
        var sharedKey = await Storage.GetItemAsync<byte[]>("shared-bin");
        return CryptoHelper.DecryptData(result, sharedKey);
    }

    /// <inheritdoc/>
    public  async UnaryResult<TModel> UpdateEncrypted(TModel model)
    {
        var sharedKey = await Storage.GetItemAsync<byte[]>("shared-bin");

        var encryptedData = CryptoHelper.EncryptData(model, sharedKey);

        var result = await Client.UpdateEncrypted(encryptedData);

        return CryptoHelper.DecryptData(result, sharedKey);
    }

    /// <inheritdoc/>
    public  async UnaryResult<TModel> DeleteEncrypted(TModel model)
    {
        var sharedKey = await Storage.GetItemAsync<byte[]>("shared-bin");

        var encryptedData = CryptoHelper.EncryptData(model, sharedKey);

        var result = await Client.DeleteEncrypted(encryptedData);

        return  CryptoHelper.DecryptData(result, sharedKey);
    }


    public async UnaryResult<List<TModel>> FindByParentEncrypted(string parentId, string foreignKey)
    {
        var sharedKey = await Storage.GetItemAsync<byte[]>("shared-bin");

        var encryptedParentId =  CryptoHelper.EncryptData(parentId, sharedKey);

        var encryptedForeignKey =  CryptoHelper.EncryptData(foreignKey, sharedKey);

        var result = await Client.FindByParentEncryptedAsync(encryptedParentId,encryptedForeignKey);

        return  CryptoHelper.DecryptData(result, sharedKey);
    }

    public async IAsyncEnumerable<List<TModel>> StreamReadAllEncypted(int  bathcSize)
    {
        var sharedKey = await Storage.GetItemAsync<byte[]>("shared-bin");
 
        var result = await Client.StreamReadAllEncypted(bathcSize);

        await foreach( var responseData in result.ResponseStream.ReadAllAsync())
        {
            var decrypted = CryptoHelper.DecryptData(responseData, sharedKey);

            yield return decrypted;
        }     
    }
}

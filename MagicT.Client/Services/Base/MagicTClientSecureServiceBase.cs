using MagicOnion;
using MagicOnion.Client;
using MagicT.Client.Models;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models.ServiceModels;
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
    public new async UnaryResult<TModel> CreateEncrypted(EncryptedData<TModel> encryptedData)
    {  
        var result = await base.CreateEncrypted(encryptedData);
        var sharedKey = await Storage.GetItemAsync<byte[]>("shared-bind");
        return await CryptionHelper.DecryptData(result, sharedKey);
    }

    /// <inheritdoc/>
    public new async UnaryResult<List<TModel>> ReadAllEncrypted()
    {
        var result = await base.ReadAllEncrypted();
        var sharedKey = await Storage.GetItemAsync<byte[]>("shared-bind");
        return await CryptionHelper.DecryptData(result, sharedKey);
    }

    /// <inheritdoc/>
    public new async UnaryResult<TModel> UpdateEncrypted(EncryptedData<TModel> encryptedData)
    {
        var result = await base.UpdateEncrypted(encryptedData);
        var sharedKey = await Storage.GetItemAsync<byte[]>("shared-bind");
        return await CryptionHelper.DecryptData(result, sharedKey);
    }

    /// <inheritdoc/>
    public new async UnaryResult<TModel> DeleteEncrypted(EncryptedData<TModel> encryptedData)
    {
        var result = await base.DeleteEncrypted(encryptedData);
        var sharedKey = await Storage.GetItemAsync<byte[]>("shared-bind");
        return await CryptionHelper.DecryptData(result, sharedKey);
    }
}

using Benutomo;
using MagicT.Shared.Models.ViewModels;
// using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Managers;

/// <summary>
/// Manages storage operations for the client.
/// </summary>
[RegisterScoped]
[AutomaticDisposeImpl]
public partial class StorageManager:IDisposable
{
    /// <summary>
    /// Gets or sets the local storage service.
    /// </summary>
    private ISessionStorageService LocalStorage { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageManager"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public StorageManager(IServiceProvider provider)
    {
        LocalStorage = provider.GetService<ISessionStorageService>();
    }
    
    ~StorageManager()
    {
        Dispose(false);
    }

    // /// <summary>
    // /// Stores the client's shared data in local storage.
    // /// </summary>
    // /// <param name="clientShared">The client's shared data.</param>
    // public async Task StoreUserSharedAsync(byte[] clientShared)
    // {
    //     if (await LocalStorage.ContainKeyAsync("shared-bin")) return;
    //     await LocalStorage.SetItemAsync("shared-bin", clientShared);
    // }
    //
    // /// <summary>
    // /// Stores the client's public data in local storage.
    // /// </summary>
    // /// <param name="publicBytes">The client's public data.</param>
    // public async Task StoreUserPublicAsync(byte[] publicBytes)
    // {
    //     if (await LocalStorage.ContainKeyAsync("public-bin")) return;
    //     await LocalStorage.SetItemAsync("public-bin", publicBytes);
    // }

    /// <summary>
    /// Stores the client's login data in local storage.
    /// </summary>
    /// <param name="request">The login request.</param>
    public async Task StoreClientLoginDataAsync(AuthenticationRequest request)
    {
        await LocalStorage.SetItemAsync(nameof(AuthenticationRequest), request);
    }

    /// <summary>
    /// Signs out the client by removing the login data from local storage.
    /// </summary>
    public async Task SignOutAsync()
    {
        await LocalStorage.RemoveItemAsync(nameof(AuthenticationRequest));
    }

    /// <summary>
    /// Retrieves the client's shared data from local storage.
    /// </summary>
    /// <returns>The client's shared data.</returns>
    public async Task<byte[]> GetSharedBytesAsync()
    {
        return await LocalStorage.GetItemAsync<byte[]>("shared-bin");
    }

    // /// <summary>
    // /// Retrieves the client's public data from local storage.
    // /// </summary>
    // /// <returns>The client's public data.</returns>
    // public async Task<byte[]> GetPublicBytesAsync()
    // {
    //     return await LocalStorage.GetItemAsync<byte[]>("public-bin");
    // }

    /// <summary>
    /// Retrieves the client's login data from local storage.
    /// </summary>
    /// <returns>The login request.</returns>
    public async Task<AuthenticationRequest> GetLoginDataAsync()
    {
        return await LocalStorage.GetItemAsync<AuthenticationRequest>(nameof(AuthenticationRequest));
    }

    /// <summary>
    /// Clears all data from local storage.
    /// </summary>
    public async Task ClearAllAsync()
    {
        await LocalStorage.ClearAsync();
    }

    /// <summary>
    /// Stores the client's token in local storage.
    /// </summary>
    /// <param name="token">The client's token.</param>
    public async Task StoreTokenAsync(byte[] token)
    {
        await LocalStorage.SetItemAsync("token-bin", token);
    }

    /// <summary>
    /// Retrieves the client's token from local storage.
    /// </summary>
    /// <returns>The client's token.</returns>
    public async Task<byte[]> GetTokenAsync()
    {
       
        var token = await LocalStorage.GetItemAsync<byte[]>("token-bin");
        return token;
    }
}
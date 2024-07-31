using MagicT.Shared.Models.ViewModels;
using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Managers;

// Attribute to register this class as a scoped service
[RegisterScoped]
public class StorageManager
{
    // Instance of the local storage service
    public ILocalStorageService localStorage { get; set; }

    // Constructor to inject dependencies
    public StorageManager(IServiceProvider provider)
    {
        localStorage = provider.GetService<ILocalStorageService>();
    }

    // Method to store the client's shared data in local storage
    public async Task StoreClientSharedAsync(byte[] ClientShared)
    {
        if (await localStorage.ContainKeyAsync("shared-bin")) return;
        await localStorage.SetItemAsync("shared-bin", ClientShared);
    }

    // Method to store the client's public data in local storage
    public async Task StoreClientPublicAsync(byte[] PublicBytes)
    {
        if (await localStorage.ContainKeyAsync("public-bin")) return;
        await localStorage.SetItemAsync("public-bin", PublicBytes);
    }

    // Method to store the client's login data in local storage
    public async Task StoreClientLoginDataAsync(LoginRequest request)
    {
        await localStorage.SetItemAsync(nameof(LoginRequest), request);
    }

    // Method to sign out the client by removing the login data from local storage
    public async Task SignOutAsync()
    {
        await localStorage.RemoveItemAsync(nameof(LoginRequest));
    }

    // Method to retrieve the client's shared data from local storage
    public async Task<byte[]> GetSharedBytesAsync()
    {
        return await localStorage.GetItemAsync<byte[]>("shared-bin");
    }

    // Method to retrieve the client's public data from local storage
    public async Task<byte[]> GetPublicBytesAsync()
    {
        return await localStorage.GetItemAsync<byte[]>("public-bin");
    }

    // Method to retrieve the client's login data from local storage
    public async Task<LoginRequest> GetLoginDataAsync()
    {
        return await localStorage.GetItemAsync<LoginRequest>(nameof(LoginRequest));
    }

    // Method to clear all data from local storage
    public async Task ClearAllAsync()
    {
        await localStorage.ClearAsync();
    }

    // Method to store the client's token in local storage
    public async Task StoreTokenAsync(byte[] Token)
    {
        await localStorage.SetItemAsync("token-bin", Token);
    }

    // Method to retrieve the client's token from local storage
    public async Task<byte[]> GetTokenAsync()
    {
        return await localStorage.GetItemAsync<byte[]>("token-bin");
    }
}
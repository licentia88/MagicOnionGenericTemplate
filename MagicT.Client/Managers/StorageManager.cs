using MagicT.Shared.Models.ViewModels;
using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Managers;

public class StorageManager: IStorageManager
{
    ILocalStorageService localStorage { get; set; }

    public StorageManager(IServiceProvider provider)
    {
        localStorage = provider.GetService<ILocalStorageService>();
    }

    public async Task StoreClientSharedAsync(byte[] ClientShared)
    {
        await localStorage.SetItemAsync("shared-bin", ClientShared);
    }

    public async Task StoreClientPublicAsync(byte[] PublicBytes)
    {
        await localStorage.SetItemAsync("public-bin", PublicBytes);
    }

    public async Task StoreClientLoginDataAsync(LoginRequest request)
    {
        await localStorage.SetItemAsync(nameof(LoginRequest), request);
    }

    public async Task<byte[]> GetSharedBytesAsync()
    {
        return await localStorage.GetItemAsync<byte[]>("shared-bin");
    }

    public async Task<byte[]> GetPublicBytesAsync()
    {
        return await localStorage.GetItemAsync<byte[]>("public-bin");
    }

    public async Task<LoginRequest> GetLoginDataAsync()
    {
        return await localStorage.GetItemAsync<LoginRequest>(nameof(LoginRequest));
    }

    public async Task ClearAllAsync()
    {
        await localStorage.ClearAsync();
    }

    public async Task StoreTokenAsync(byte[] Token)
    {
        await localStorage.SetItemAsync("token-bin", Token);
    }

    public async Task<byte[]> GetTokenAsync()
    {
       return  await localStorage.GetItemAsync<byte[]>("token-bin");
    }
}

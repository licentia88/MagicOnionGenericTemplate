using MagicT.Client.Models;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Models.ViewModels;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using MessagePipe;

namespace MagicT.Web.Managers;

public class UserManager
{
    public ILocalStorageService LocalStorageService { get; set; }
    public MagicTClientData MagicTClientData { get; set; }

    public IPublisher<(string Identifier, EncryptedData<string> SecurePassword)> LoginPublisher { get; set; }

    public UserManager(IServiceProvider provider)
    {
        LocalStorageService = provider.GetService<ILocalStorageService>();
        MagicTClientData = provider.GetService<MagicTClientData>();
        LoginPublisher = provider.GetService<IPublisher<(string Identifier, EncryptedData<string> SecurePassword)>>();

    }

    public Task<byte[]> GetSharedKeyAsync()
    {
        return LocalStorageService.GetItemAsync<byte[]>("shared-bin");
    }
    
    public Task<byte[]> GetTokenAsync()
    {
        return LocalStorageService.GetItemAsync<byte[]>("token-bin");
    }
    
    public async Task<(string Identifier,EncryptedData<string> SecurePassword)> GetLoginDataAsync()
    {
        var identifier = await LocalStorageService.GetItemAsync<string>("Identifier");
        var securePassword = await LocalStorageService.GetItemAsync<EncryptedData<string>>("securePassword");


        return (identifier, securePassword);
    }
    
    public async Task SignInAsync(LoginRequest loginRequest)
    {
        var sharedKey = await GetSharedKeyAsync();
        var securePassword = CryptoHelper.EncryptData(loginRequest.Password, sharedKey);

        await LocalStorageService.SetItemAsync(nameof(securePassword), securePassword);
        await LocalStorageService.SetItemAsync(nameof(loginRequest.Identifier), loginRequest.Identifier);

        LoginPublisher.Publish((loginRequest.Identifier, securePassword));
    }
    
    public async Task SignOutAsync()
    {
        await LocalStorageService.RemoveItemAsync("shared-bin");
        await LocalStorageService.RemoveItemAsync("public-bin");
        await LocalStorageService.RemoveItemAsync("token-bin");
        await LocalStorageService.RemoveItemAsync("securePassword");
        await LocalStorageService.RemoveItemAsync("Identifier");
    }
}
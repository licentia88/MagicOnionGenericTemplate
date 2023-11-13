using MagicT.Shared.Models.ViewModels;

namespace MagicT.Client.Managers;

public interface IStorageManager
{
    public Task StoreClientSharedAsync(byte[] ClientShared);

    public Task StoreTokenAsync(byte[] Token);

    public Task StoreClientPublicAsync(byte[] PublicBytes);

    public Task StoreClientLoginDataAsync(LoginRequest request);

    public Task<byte[]> GetSharedBytesAsync();

    public Task<byte[]> GetPublicBytesAsync();

    public Task<LoginRequest> GetLoginDataAsync();

    public Task<byte[]> GetTokenAsync();

    public Task ClearAllAsync();



}

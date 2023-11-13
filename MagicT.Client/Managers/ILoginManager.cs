using MagicT.Shared.Models.ViewModels;
using MessagePipe;
using MagicT.Shared.Managers;
using Org.BouncyCastle.Crypto;

namespace MagicT.Client.Managers;

public interface ILoginManager
{
    public Task Initialize();

    byte[] ClientShared { get; set; }

    public LoginRequest LoginData { get; set; }

    IStorageManager StorageManager { get; set; }

    IKeyExchangeManager KeyExchangeManager { get; set; }

    public IPublisher<LoginRequest> LoginPublisher { get; set; }

    public ISubscriber<LoginRequest> LoginSubscriber { get; set; }

    (byte[] PublicBytes, AsymmetricKeyParameter PrivateKey) ClientKeys { get; set; }

 
    Task SignOutAsync();

    Task SignInAsync(LoginRequest loginRequest);

    public Task<byte[]> CreateAndStoreUserPublics();
}

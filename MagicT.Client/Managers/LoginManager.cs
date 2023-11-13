using MagicT.Client.Models;
using MagicT.Shared.Models.ViewModels;
using MessagePipe;
using MagicT.Shared.Managers;
using Org.BouncyCastle.Crypto;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Managers;

public class LoginManager : ILoginManager
{
    public IStorageManager StorageManager { get; set; }

    public IKeyExchangeManager KeyExchangeManager { get; set; }

    public byte[] ClientShared { get; set; }

    public (byte[] PublicBytes, AsymmetricKeyParameter PrivateKey) ClientKeys { get; set; }

    public MagicTClientData MagicTClientData { get; set; }

    public IPublisher<LoginRequest> LoginPublisher { get; set; }

    public ISubscriber<LoginRequest> LoginSubscriber { get; set; }

    //public bool IsSignedIn { get; set; }

    public LoginRequest LoginData { get; set; }

    public LoginManager(IServiceProvider provider)
    {
        KeyExchangeManager = provider.GetService<IKeyExchangeManager>();

        StorageManager = provider.GetService<IStorageManager>();

        MagicTClientData = provider.GetService<MagicTClientData>();

        LoginPublisher = provider.GetService<IPublisher<LoginRequest>>();

        LoginSubscriber = provider.GetService<ISubscriber<LoginRequest>>();
    }
 

    public async Task SignInAsync(LoginRequest loginRequest)
    {
        await StorageManager.StoreClientLoginDataAsync(loginRequest);
 
        LoginPublisher.Publish(loginRequest);
    }

    public async Task SignOutAsync()
    {
        await StorageManager.ClearAllAsync();

    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<byte[]> CreateAndStoreUserPublics()
    {
        //Create client's public key bytes and private key
        ClientKeys = KeyExchangeManager.CreatePublicKey();

        //Create shared key from server's public key and store it in LocalStorage
        ClientShared = KeyExchangeManager.CreateSharedKey(KeyExchangeManager.KeyExchangeData.OtherPublicBytes, ClientKeys.PrivateKey);

        //Store shared key in LocalStorage for data encryption
        await StorageManager.StoreClientSharedAsync(ClientShared);

        //Store client's public key in LocalStorage for sending to server on login or register
        await StorageManager.StoreClientPublicAsync(ClientKeys.PublicBytes);

        return ClientShared;
    }
  
    public async Task Initialize()
    {
        LoginData = await StorageManager.GetLoginDataAsync();
        //if (!IsSignedIn)
            
    }
}
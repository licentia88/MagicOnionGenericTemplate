using MagicT.Client.Models;
using MagicT.Shared.Models.ViewModels;
using MessagePipe;
using MagicT.Shared.Managers;
using Org.BouncyCastle.Crypto;
using Microsoft.Extensions.DependencyInjection;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Cryptography;

namespace MagicT.Client.Managers;

[RegisterScoped]
public class LoginManager
{
    // Dependency properties
    public StorageManager StorageManager { get; set; }
    public IKeyExchangeManager KeyExchangeManager { get; set; }
    public byte[] ClientShared { get; set; }
    public (byte[] PublicBytes, AsymmetricKeyParameter PrivateKey) ClientKeys { get; set; }
    public MagicTClientData MagicTClientData { get; set; }
    public IPublisher<LoginRequest> LoginPublisher { get; set; }
    public ISubscriber<LoginRequest> LoginSubscriber { get; set; }
    public IDistributedSubscriber<string, EncryptedData<byte[]>> TokenSubscriber { get; set; }

    // Login data property
    public LoginRequest LoginData { get; set; }

    // Constructor to inject dependencies
    public LoginManager(IServiceProvider provider)
    {
        KeyExchangeManager = provider.GetService<IKeyExchangeManager>();
        StorageManager = provider.GetService<StorageManager>();
        MagicTClientData = provider.GetService<MagicTClientData>();
        LoginPublisher = provider.GetService<IPublisher<LoginRequest>>();
        LoginSubscriber = provider.GetService<ISubscriber<LoginRequest>>();
        TokenSubscriber = provider.GetService<IDistributedSubscriber<string, EncryptedData<byte[]>>>();
    }

    // Method to initiate the sign-in process
    public async Task SignInAsync(LoginRequest loginRequest)
    {
        await StorageManager.StoreClientLoginDataAsync(loginRequest);
        LoginPublisher.Publish(loginRequest);
    }

    // Method to handle token refresh subscription
    public async Task TokenRefreshSubscriber(LoginRequest loginRequest)
    {
        await TokenSubscriber.SubscribeAsync(loginRequest.Identifier.ToUpper(), async encryptedData =>
        {
            var decryptedToken = CryptoHelper.DecryptData(encryptedData, ClientShared);
            await StorageManager.StoreTokenAsync(decryptedToken);
        });
    }

    // Method to sign out the user
    public async Task SignOutAsync()
    {
        await StorageManager.SignOutAsync();
        LoginData = null;
    }

    // Method to create and store the user's public key and shared key
    public async Task<byte[]> CreateAndStoreUserPublics()
    {
        // Create client's public key bytes and private key
        ClientKeys = KeyExchangeManager.CreatePublicKey();

        // Create shared key from server's public key and store it in LocalStorage
        ClientShared = KeyExchangeManager.CreateSharedKey(KeyExchangeManager.KeyExchangeData.OtherPublicBytes, ClientKeys.PrivateKey);

        // Store shared key in LocalStorage for data encryption
        await StorageManager.StoreClientSharedAsync(ClientShared);

        // Store client's public key in LocalStorage for sending to server on login or register
        await StorageManager.StoreClientPublicAsync(ClientKeys.PublicBytes);

        return ClientShared;
    }

    // Method to initialize the LoginManager
    public async Task Initialize()
    {
        LoginData = await StorageManager.GetLoginDataAsync();
    }
}
using MagicT.Client.Models;
using MagicT.Shared.Models.ViewModels;
using MessagePipe;
using MagicT.Shared.Managers;
using Org.BouncyCastle.Crypto;
using Microsoft.Extensions.DependencyInjection;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Cryptography;

namespace MagicT.Client.Managers;

/// <summary>
/// Manages the login process for the client.
/// </summary>
[RegisterScoped]
public class LoginManager
{
    /// <summary>
    /// Gets or sets the storage manager.
    /// </summary>
    public StorageManager StorageManager { get; set; }

    /// <summary>
    /// Gets or sets the key exchange manager.
    /// </summary>
    private IKeyExchangeManager KeyExchangeManager { get; set; }

    /// <summary>
    /// Gets or sets the client shared key.
    /// </summary>
    public byte[] ClientShared { get; set; }

    /// <summary>
    /// Gets or sets the client keys.
    /// </summary>
    public (byte[] PublicBytes, AsymmetricKeyParameter PrivateKey) ClientKeys { get; set; }

    /// <summary>
    /// Gets or sets the client data.
    /// </summary>
    public MagicTClientData MagicTClientData { get; set; }

    /// <summary>
    /// Gets or sets the login request publisher.
    /// </summary>
    public IPublisher<LoginRequest> LoginPublisher { get; set; }

    /// <summary>
    /// Gets or sets the login request subscriber.
    /// </summary>
    public ISubscriber<LoginRequest> LoginSubscriber { get; set; }

    /// <summary>
    /// Gets or sets the token subscriber.
    /// </summary>
    public IDistributedSubscriber<string, EncryptedData<byte[]>> TokenSubscriber { get; set; }

    /// <summary>
    /// Gets or sets the login data.
    /// </summary>
    public LoginRequest LoginData { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginManager"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public LoginManager(IServiceProvider provider)
    {
        KeyExchangeManager = provider.GetService<IKeyExchangeManager>();
        StorageManager = provider.GetService<StorageManager>();
        MagicTClientData = provider.GetService<MagicTClientData>();
        LoginPublisher = provider.GetService<IPublisher<LoginRequest>>();
        LoginSubscriber = provider.GetService<ISubscriber<LoginRequest>>();
        TokenSubscriber = provider.GetService<IDistributedSubscriber<string, EncryptedData<byte[]>>>();
    }

    /// <summary>
    /// Initiates the sign-in process.
    /// </summary>
    /// <param name="loginRequest">The login request.</param>
    public async Task SignInAsync(LoginRequest loginRequest)
    {
        await StorageManager.StoreClientLoginDataAsync(loginRequest);
        LoginPublisher.Publish(loginRequest);
    }

    /// <summary>
    /// Handles token refresh subscription.
    /// </summary>
    /// <param name="loginRequest">The login request.</param>
    public async Task TokenRefreshSubscriber(LoginRequest loginRequest)
    {
        await TokenSubscriber.SubscribeAsync(loginRequest.Identifier.ToUpper(), async encryptedData =>
        {
            var decryptedToken = CryptoHelper.DecryptData(encryptedData, ClientShared);
            await StorageManager.StoreTokenAsync(decryptedToken);
        });
    }

    /// <summary>
    /// Signs out the user.
    /// </summary>
    public async Task SignOutAsync()
    {
        await StorageManager.SignOutAsync();
        await StorageManager.ClearAllAsync();
        await CreateAndStoreUserPublics();
        LoginData = null;
    }

    /// <summary>
    /// Creates and stores the user's public key and shared key.
    /// </summary>
    /// <returns>The client shared key.</returns>
    public async Task CreateAndStoreUserPublics()
    {
        ClientKeys = KeyExchangeManager.CreatePublicKey();
        ClientShared = KeyExchangeManager.CreateSharedKey(KeyExchangeManager.KeyExchangeData.OtherPublicBytes, ClientKeys.PrivateKey);

        await StorageManager.StoreClientSharedAsync(ClientShared);
        await StorageManager.StoreClientPublicAsync(ClientKeys.PublicBytes);
    }

    /// <summary>
    /// Initializes the LoginManager.
    /// </summary>
    public async Task Initialize()
    {
        LoginData = await StorageManager.GetLoginDataAsync();
    }
}
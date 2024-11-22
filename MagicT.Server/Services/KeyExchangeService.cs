using Benutomo;
using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Managers;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

/// <summary>
/// Service for handling key exchange operations with encryption and authorization.
/// </summary>
[AutomaticDisposeImpl]
// ReSharper disable once UnusedType.Global
public  partial class KeyExchangeService : MagicServerService<IKeyExchangeService, byte[], MagicTContext>, IKeyExchangeService, IDisposable
{
    /// <summary>
    /// Gets or sets the key exchange manager.
    /// </summary>
    private IKeyExchangeManager KeyExchangeManager { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyExchangeService"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public KeyExchangeService(IServiceProvider provider) : base(provider)
    {
        KeyExchangeManager = provider.GetRequiredService<IKeyExchangeManager>();
    }
    
    ~KeyExchangeService()
    {
        Dispose(false);
    }
    
    /// <summary>
    /// Performs a global key exchange asynchronously.
    /// </summary>
    /// <param name="clientPublic">The client's public key.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the server's public key.</returns>
    public UnaryResult<byte[]> GlobalKeyExchangeAsync(byte[] clientPublic)
    {
        KeyExchangeManager.KeyExchangeData.SharedBytes = KeyExchangeManager.CreateSharedKey(clientPublic, KeyExchangeManager.KeyExchangeData.PrivateKey);

        return new UnaryResult<byte[]>(KeyExchangeManager.KeyExchangeData.SelfPublicBytes);
    }
}
using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Helpers;
using MagicT.Shared.Managers;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services;
using Org.BouncyCastle.Crypto;

namespace MagicT.Server.Services;

// ReSharper disable once UnusedType.Global
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class KeyExchangeService : MagicServerService<IKeyExchangeService, byte[]>, IKeyExchangeService
{
    public IKeyExchangeManager KeyExchangeManager { get; set; }


    public KeyExchangeService(IServiceProvider provider) : base(provider)
    {
        KeyExchangeManager = provider.GetRequiredService<IKeyExchangeManager>();
    }


    public UnaryResult<byte[]> GlobalKeyExchangeAsync(byte[] clientPublic)
    {
        KeyExchangeManager.KeyExchangeData.SharedBytes = KeyExchangeManager.CreateSharedKey(clientPublic, KeyExchangeManager.KeyExchangeData.PrivateKey);

        return new UnaryResult<byte[]>(KeyExchangeManager.KeyExchangeData.SelfPublicBytes);
    }
 
}
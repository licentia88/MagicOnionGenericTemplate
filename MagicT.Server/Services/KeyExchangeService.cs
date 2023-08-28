using MagicOnion;
using MagicT.Client.Models;
using MagicT.Server.Services.Base;
using MagicT.Shared.Helpers;
using MagicT.Shared.Services;
using Org.BouncyCastle.Crypto;

namespace MagicT.Server.Services;

// ReSharper disable once UnusedType.Global
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class KeyExchangeService : MagicServerServiceBase<IKeyExchangeService, byte[]>, IKeyExchangeService
{
    public GlobalData GlobalData { get; set; }

    public static (byte[] publicKeyBytes, AsymmetricKeyParameter privateKey) PublicKeyData;
    
    public KeyExchangeService(IServiceProvider provider) : base(provider)
    {
        GlobalData = provider.GetRequiredService<GlobalData>();
    }

    public UnaryResult<byte[]> GlobalKeyExchangeAsync(byte[] clientPublic)
    {
        CreatePublicKeyData();

        GlobalData.Shared = DiffieHellmanKeyExchange.CreateSharedKey(clientPublic, PublicKeyData.privateKey);

        return new UnaryResult<byte[]>(PublicKeyData.publicKeyBytes);
    }


    /// <summary>
    /// Creates a Public Key and sends it to the client
    /// </summary>
    /// <returns></returns>
    public UnaryResult<byte[]> RequestServerPublicKeyAsync()
    {
        if (PublicKeyData.publicKeyBytes is not null) 
            return new UnaryResult<byte[]>(PublicKeyData.publicKeyBytes);

        CreatePublicKeyData();

        //Return public key to client to create shared key
        return new UnaryResult<byte[]>(PublicKeyData.publicKeyBytes);
    }

    private void CreatePublicKeyData()
    {
        PublicKeyData = DiffieHellmanKeyExchange.CreatePublicKey();
    }
}
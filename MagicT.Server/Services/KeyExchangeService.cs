using System.Security.Cryptography;
using MagicOnion;
using MagicT.Server.Services.Base;
using MagicT.Shared.Helpers;
using MagicT.Shared.Services;
using Org.BouncyCastle.Crypto;

namespace MagicT.Server.Services;

// ReSharper disable once UnusedType.Global
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class KeyExchangeService : MagicTServerServiceBase<IKeyExchangeService, byte[]>, IKeyExchangeService
{
    public static (byte[] publicKeyBytes, AsymmetricKeyParameter privateKey) PublicKeyData;
    
    public KeyExchangeService(IServiceProvider provider) : base(provider)
    {
        
    }


    /// <summary>
    /// Creates a Public Key and sends it to the client
    /// </summary>
    /// <returns></returns>
    public UnaryResult<byte[]> RequestServerPublicKeyAsync()
    {
        if (PublicKeyData.publicKeyBytes is not null) 
            return new UnaryResult<byte[]>(PublicKeyData.publicKeyBytes);
        
        PublicKeyData = DiffieHellmanKeyExchange.CreatePublicKey();

        //Return public key to client to create shared key
        return new UnaryResult<byte[]>(PublicKeyData.publicKeyBytes);
    }
}
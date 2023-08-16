using System.Security.Cryptography;
using MagicOnion;
using MagicT.Server.Services.Base;
using MagicT.Shared.Helpers;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

// ReSharper disable once UnusedType.Global
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class KeyExchangeService : MagicTServerServiceBase<IKeyExchangeService, byte[]>, IKeyExchangeService
{
    private static byte[] PublicKey;

    public KeyExchangeService(IServiceProvider provider) : base(provider)
    {
        
    }


    /// <summary>
    /// Creates a Public Key and sends it to the client
    /// </summary>
    /// <returns></returns>
    public UnaryResult<byte[]> RequestServerPublicKeyAsync()
    {
        if (PublicKey is not null) return new UnaryResult<byte[]>(PublicKey);

 
        byte[] serverPublicKey = DiffieHellmanKeyExchange.CreatePublicKey();

        //Return public key to client to create shared key
        return new UnaryResult<byte[]>(serverPublicKey);
    }
}
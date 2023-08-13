using System.Security.Cryptography;
using MagicOnion;
using MagicT.Server.Services.Base;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models.MemoryDatabaseModels;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

// ReSharper disable once UnusedType.Global
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class KeyExchangeService : MagicTServerServiceBase<IKeyExchangeService, byte[]>, IKeyExchangeService
{
    public KeyExchangeService(IServiceProvider provider) : base(provider)
    {
        
    }


    /// <summary>
    /// Creates a Public Key and sends it to the client
    /// </summary>
    /// <returns></returns>
    public UnaryResult<byte[]> RequestServerPublicKeyAsync()
    {

        var Usrs = new Users[]
        {
             new(){ UserId = 1}
        };

        var builder = MemoryDatabase.ToImmutableBuilder();

        builder.Diff(Usrs);

        MemoryDatabase =builder.Build();

        var test = MemoryDatabase.UsersTable.Count;

        var test2 = MemoryDatabase.UsersTable.FindByUserId(1);


        using ECDiffieHellmanCng serviceDH = new();

        byte[] serverPublicKey = serviceDH.CreatePublicKey();

        //Return public key to client to create shared key
        return new UnaryResult<byte[]>(serverPublicKey);
    }
}
using System.Security.Cryptography;
using Grpc.Core;
using MagicOnion;
using MagicT.Server.Services.Base;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public class DiffieHellmanKeyExchangeService : MagicBase<IDiffieHellmanKeyExchangeService, byte[]>, IDiffieHellmanKeyExchangeService
{
    public DiffieHellmanKeyExchangeService(IServiceProvider provider) : base(provider)
    {
    }

    public async Task<DuplexStreamingResult<byte[], byte[]>> InitializeKeyExchangeAsync()
    {
        // DuplexStreamingContext represents both server and client streaming.
        var stream = GetDuplexStreamingContext<byte[], byte[]>();

        using ECDiffieHellmanCng serviceDH = new();

        byte[] servicePublicKey = serviceDH.ExportSubjectPublicKeyInfo();

        await stream.WriteAsync(servicePublicKey);
        // Send servicePublicKey to the client.

        // Receive clientPublicKey from the client.
        await foreach (var clientPublicKey in stream.ReadAllAsync())
        {
            byte[] sharedSecret = serviceDH.DeriveKeyMaterial(CngKey.Import(clientPublicKey, CngKeyBlobFormat.EccPublicBlob));
        }
        return stream.Result();
        // Now you can use the sharedSecret to encrypt/decrypt data or generate symmetric keys.
    }
}
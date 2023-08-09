using System.Security.Cryptography;
using Grpc.Core;
using MagicOnion;
using MagicT.Client.Filters;
using MagicT.Client.Services.Base;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

public class DiffieHellmanKeyExchangeService : ServiceBase<IDiffieHellmanKeyExchangeService, byte[]>, IDiffieHellmanKeyExchangeService
{
    public DiffieHellmanKeyExchangeService(IServiceProvider provider) : base(provider, new AuthenticationFilter(provider))
    {
    }

    

    public async Task<DuplexStreamingResult<byte[], byte[]>> InitializeKeyExchangeAsync()
    {

        var stream = await Client.InitializeKeyExchangeAsync();

        using ECDiffieHellmanCng clientDH = new();

        var clientPublicKey = clientDH.ExportSubjectPublicKeyInfo();

        // Send clientPublicKey to the service.
        await stream.RequestStream.WriteAsync(clientPublicKey);

        await foreach (var servicePublicKey in stream.ResponseStream.ReadAllAsync())
        {
            byte[] sharedSecret = clientDH.DeriveKeyMaterial(CngKey.Import(servicePublicKey, CngKeyBlobFormat.EccPublicBlob));
        }

        // Now you can use the sharedSecret to encrypt/decrypt data or generate symmetric keys.
        await stream.RequestStream.CompleteAsync();

        return default;
    }
}
using MagicOnion;
using MagicT.Shared.Services.Base;

namespace MagicT.Shared.Services;

 /// <summary>
 /// Using Diffie Hellman
 /// </summary>
public interface IDiffieHellmanKeyExchangeService : IGenericService<IDiffieHellmanKeyExchangeService, byte[]>
{
    public Task<DuplexStreamingResult<byte[], byte[]>> InitializeKeyExchangeAsync();
}
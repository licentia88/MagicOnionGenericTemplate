using MagicOnion;
using MagicT.Shared.Services.Base;

namespace MagicT.Shared.Services;

 /// <summary>
 /// Using Diffie Hellman
 /// </summary>
public interface IKeyExchangeService : IMagicTService<IKeyExchangeService, byte[]>
{
    public UnaryResult<byte[]> RequestServerPublicKeyAsync();

    UnaryResult<byte[]> GlobalKeyExchangeAsync(byte[] clientPublic);

}
using MagicOnion;
using MagicT.Shared.Services.Base;

namespace MagicT.Shared.Services;

 /// <summary>
 /// Using Diffie Hellman
 /// </summary>
public interface IKeyExchangeService : IMagicService<IKeyExchangeService, byte[]>
{
    //public UnaryResult<byte[]> RequestServerPublicKeyAsync();

    UnaryResult<byte[]> GlobalKeyExchangeAsync(byte[] clientPublic);

    //public UnaryResult<byte[]> TTATATATATA();

}
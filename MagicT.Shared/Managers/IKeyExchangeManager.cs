using MagicT.Shared.Models.ServiceModels;
using Org.BouncyCastle.Crypto;

namespace MagicT.Shared.Managers;

public interface IKeyExchangeManager
{
	public KeyExchangeData KeyExchangeData { get; set; }

	(byte[] PublicBytes, AsymmetricKeyParameter PrivateKey) CreatePublicKey();

	byte[] CreateSharedKey(byte[] PublicBytes, AsymmetricKeyParameter PrivateKey);

}

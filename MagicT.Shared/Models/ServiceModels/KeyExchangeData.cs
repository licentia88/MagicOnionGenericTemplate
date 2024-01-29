using Org.BouncyCastle.Crypto;

namespace MagicT.Shared.Models.ServiceModels;

[RegisterSingleton]
public class KeyExchangeData
{
    public byte[] SharedBytes { get; set; }

    public byte[] SelfPublicBytes { get; set; }

    public byte[] OtherPublicBytes { get; set; }

    public AsymmetricKeyParameter PrivateKey { get; set; }
}

using MemoryPack;

namespace MagicT.Server.Models;

[MemoryPackable]
public partial class UsedTokens
{
    public byte[] EncryptedBytes { get; set; }

    public byte[] Nonce { get; set; }

    public byte[] Mac { get; set; }

    public UsedTokens(byte[] encryptedBytes, byte[] nonce, byte[] mac)
    {
        EncryptedBytes = encryptedBytes;
        Nonce = nonce;
        Mac = mac;
    }

}

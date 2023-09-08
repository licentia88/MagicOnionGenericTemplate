using MemoryPack;

namespace MagicT.Server.ZoneTree.Models;

[MemoryPackable]
public partial class UsedTokensZone
{
    public byte[] EncryptedBytes { get; set; }

    public byte[] Nonce { get; set; }

    public byte[] Mac { get; set; }

    public UsedTokensZone(byte[] encryptedBytes, byte[] nonce, byte[] mac)
    {
        EncryptedBytes = encryptedBytes;
        Nonce = nonce;
        Mac = mac;
    }

}

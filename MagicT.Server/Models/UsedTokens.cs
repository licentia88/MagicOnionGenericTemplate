using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MemoryPack;

namespace MagicT.Server.Models;

[MemoryPackable]
public partial class UsedTokens
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int UserId { get; set; }

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

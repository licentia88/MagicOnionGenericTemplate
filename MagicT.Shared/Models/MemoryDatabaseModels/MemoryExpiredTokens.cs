using MasterMemory;
using MessagePack;

namespace MagicT.Shared.Models.MemoryDatabaseModels;

[MemoryTable(nameof(MemoryExpiredTokens)), MessagePackObject(true)]

public partial class MemoryExpiredTokens
{
    [PrimaryKey]
    public Guid id { get; set; }

    [SecondaryKey(1), NonUnique]
    public string ContactIdentifier { get; set; }

    public byte[] AssociatedToken { get; set; }
 
    public byte[] EncryptedBytes { get; set; }
   
    public byte[] Nonce { get; set; }
  
    public byte[] Mac { get; set; }

    public MemoryExpiredTokens(string contactIdentifier, byte[] associatedToken, byte[] encryptedBytes, byte[] nonce, byte[] mac)
    {
        ContactIdentifier = contactIdentifier;
        AssociatedToken = associatedToken;
        EncryptedBytes = encryptedBytes;
        Nonce = nonce;
        Mac = mac;

        id = Guid.NewGuid();
    }

}
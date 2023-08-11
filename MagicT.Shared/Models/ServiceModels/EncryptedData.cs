using Generator.Equals;
using MemoryPack;

namespace MagicT.Shared.Models.ServiceModels;

[Equatable]
[MemoryPackable]
public sealed partial  class EncryptedData<TModel>
{
    public byte[] EncryptedBytes { get; set; }
    public byte[] Nonce { get; set; }
    public byte[] AuthenticationTag { get; set; }

    public EncryptedData(byte[] encryptedBytes, byte[] nonce, byte[] authenticationTag)
    {
        EncryptedBytes = encryptedBytes;
        Nonce = nonce;
        AuthenticationTag = authenticationTag;
    }
}

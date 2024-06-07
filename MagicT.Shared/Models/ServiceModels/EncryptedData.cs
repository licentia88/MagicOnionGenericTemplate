namespace MagicT.Shared.Models.ServiceModels;

[Equatable]
[MemoryPackable]
public sealed partial  class EncryptedData<TModel>
{
    public byte[] EncryptedBytes { get; set; }
    public byte[] Nonce { get; set; }
    public byte[] Mac { get; set; }

    public EncryptedData(byte[] encryptedBytes, byte[] nonce, byte[] mac)
    {
        EncryptedBytes = encryptedBytes;
        Nonce = nonce;
        Mac = mac;
    }
}

[MemoryPackable]
public sealed partial class AuthenticationData
{
    public byte[] Token { get; set; }

    public string ContactIdentifier { get; set; }

    public AuthenticationData(byte[] token, string contactIdentifier)
    {
        Token = token;
        ContactIdentifier = contactIdentifier;
    }
}



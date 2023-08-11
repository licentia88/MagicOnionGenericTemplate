using System.Security.Cryptography;

namespace MagicT.Shared.Helpers;

public static class KeyExchangeHelper
{
    public static byte[] CreatePublicKey(this ECDiffieHellmanCng edh)
    {
        return edh.ExportSubjectPublicKeyInfo();
    }

    public static byte[] CreateSharedKey(this ECDiffieHellmanCng edh, byte[] publicKey)
    {
        return edh.DeriveKeyMaterial(CngKey.Import(publicKey, CngKeyBlobFormat.EccPublicBlob));
    }
}

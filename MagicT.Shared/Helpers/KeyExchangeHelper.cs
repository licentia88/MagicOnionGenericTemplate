using System.Security.Cryptography;

namespace MagicT.Shared.Helpers;

public static class KeyExchangeHelper
{
    // The ToByteArray() method on ECDiffieHellmanCng.PublicKey is deprecated. 
    // Microsoft recommends using ExportSubjectPublicKeyInfo() instead for future compatibility.

    // ToByteArray() is faster but may stop working in future .NET versions.
    // ExportSubjectPublicKeyInfo() has broader compatibility but lower performance.

    // To use the recommended approach:
    // 1. Comment out the ToByteArray() method call in CreatePublicKey()
    // 2. Uncomment the ExportSubjectPublicKeyInfo() method call 
    // 3. Update the CreateSharedKey() method to accept the SubjectPublicKeyInfo format.

    // This will ensure compatibility at the cost of some speed.
    // Keep ToByteArray() for better performance if compatibility is not critical.


    /// <summary>
    /// Exports the ECDiffieHellman public key.
    /// </summary>
    /// <param name="edh">The ECDiffieHellmanCng instance</param>
    /// <returns>The public key as a byte array</returns>
    /// <remarks>
    /// <para>
    /// This uses the <c>PublicKey.ToByteArray()</c> method which is deprecated. 
    /// For better compatibility, consider using <c>ExportSubjectPublicKeyInfo()</c> instead.
    /// </para>
    /// </remarks>
    public static byte[] CreatePublicKey(this ECDiffieHellmanCng edh)
    {
       

        //return edh.TryExportSubjectPublicKeyInfo();
        return edh.PublicKey.ToByteArray();
    }

    /// <summary>
    /// Derives a shared key from the local private key and remote public key.
    /// </summary>
    /// <param name="edh">The ECDiffieHellmanCng instance.</param>
    /// <param name="publicKey">The remote public key.</param>
    /// <returns>The shared secret key as a byte array.</returns>
    /// <remarks>
    /// <para>
    /// This imports the public key using <c>CngKeyBlobFormat.EccPublicBlob</c> for raw public key bytes.
    /// Alternatively, <c>CngKeyBlobFormat.GenericPublicBlob</c> can be used for SubjectPublicKeyInfo.
    /// </para>
    /// </remarks>  
    public static byte[] CreateSharedKey(this ECDiffieHellmanCng edh, byte[] publicKey)
    {
        //return edh.DeriveKeyMaterial(CngKey.Import(publicKey, CngKeyBlobFormat.GenericPublicBlob));
        return edh.DeriveKeyMaterial(CngKey.Import(publicKey, CngKeyBlobFormat.EccPublicBlob));
    }
}

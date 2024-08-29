using System.Collections;
using MagicT.Shared.Models.ServiceModels;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace MagicT.Shared.Cryptography;

/// <summary>
/// Provides cryptographic operations for data encryption, decryption, and MAC computation.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class CryptoHelper
{
    private static readonly GcmBlockCipher Cipher = new GcmBlockCipher(new AesEngine());

    /// <summary>
    /// Encrypts the provided data using AES-GCM encryption and a shared secret.
    /// </summary>
    /// <typeparam name="TModel">The type of data to encrypt.</typeparam>
    /// <param name="data">The data to encrypt.</param>
    /// <param name="sharedSecret">The shared secret (encryption key).</param>
    /// <returns>The encrypted data as an EncryptedData object.</returns>
    public static EncryptedData<TModel> EncryptData<TModel>(TModel data, byte[] sharedSecret)
    {
        var encryptedMetadata = EncryptWithMetaData(data, sharedSecret);

        return new EncryptedData<TModel>(encryptedMetadata.encryptedBytes, encryptedMetadata.Nonce, encryptedMetadata.Mac);
    }

    /// <summary>
    /// Encrypts the provided data using AES-GCM encryption and a shared secret.
    /// </summary>
    /// <typeparam name="TModel">The type of data to encrypt.</typeparam>
    /// <param name="data">The data to encrypt.</param>
    /// <param name="sharedSecret">The shared secret (encryption key).</param>
    /// <returns>A tuple containing encrypted bytes, nonce, and MAC.</returns>
    public static (byte[] encryptedBytes, byte[] Nonce, byte[] Mac) EncryptWithMetaData<TModel>(TModel data, byte[] sharedSecret)
    {
        SecureRandom random = new SecureRandom();

        // Generate a random nonce
        byte[] nonce = new byte[12];
        random.NextBytes(nonce);

        // Serialize the data
        //using MemoryStream memoryStream = new();
        var serializedData = MemoryPackSerializer.Serialize(data);

        // Encrypt the data using AES-GCM
        AeadParameters parameters = new AeadParameters(new KeyParameter(sharedSecret), 128, nonce);
        Cipher.Init(true, parameters);

        byte[] encryptedBytes = new byte[Cipher.GetOutputSize(serializedData.Length)];
        int len = Cipher.ProcessBytes(serializedData, 0, serializedData.Length, encryptedBytes, 0);
        Cipher.DoFinal(encryptedBytes, len);

        return (encryptedBytes, nonce, Cipher.GetMac());
    }


    /// <summary>
    /// Decrypts the provided encrypted data using AES-GCM decryption and a shared secret.
    /// </summary>
    /// <typeparam name="TModel">The type of data to decrypt.</typeparam>
    /// <param name="encryptedData">The encrypted data to decrypt.</param>
    /// <param name="sharedSecret">The shared secret (encryption key).</param>
    /// <returns>The decrypted data of type TModel.</returns>
    public static TModel DecryptData<TModel>(EncryptedData<TModel> encryptedData, byte[] sharedSecret)
    {
        return DecryptWithMetaData<TModel>(encryptedData.EncryptedBytes, encryptedData.Nonce, encryptedData.Mac, sharedSecret);
    }


    /// <summary>
    /// Decrypts the provided encrypted data using AES-GCM decryption and a shared secret.
    /// </summary>
    /// <typeparam name="TModel">The type of data to decrypt.</typeparam>
    /// <param name="encryptedData">The encrypted data to decrypt.</param>
    /// <param name="nonce">The nonce used for encryption.</param>
    /// <param name="mac">The MAC value for data integrity.</param>
    /// <param name="sharedSecret">The shared secret (encryption key).</param>
    /// <returns>The decrypted data of type TModel.</returns>
    public static TModel DecryptWithMetaData<TModel>(byte[] encryptedData, byte[] nonce, byte[] mac, byte[] sharedSecret)
    {
        // Decrypt the data using AES-GCM
        AeadParameters parameters = new AeadParameters(new KeyParameter(sharedSecret), 128, nonce);
        Cipher.Init(false, parameters);

        byte[] decryptedData = new byte[Cipher.GetOutputSize(encryptedData.Length)];
        int len = Cipher.ProcessBytes(encryptedData, 0, encryptedData.Length, decryptedData, 0);
        Cipher.DoFinal(decryptedData, len);

        // Verify MAC
        if (!StructuralComparisons.StructuralEqualityComparer.Equals(mac, Cipher.GetMac()))
        {
            throw new ArgumentException("MAC verification failed. Data may have been tampered with.");
        }

        // Deserialize the decrypted data
        return MemoryPackSerializer.Deserialize<TModel>(decryptedData);
    }


}

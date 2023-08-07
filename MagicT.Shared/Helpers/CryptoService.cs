using System.Collections;
using System.Security.Cryptography;
using MemoryPack;

namespace MagicT.Shared.Helpers;

/// <summary>
/// Provides cryptographic operations for data encryption, decryption, and MAC computation.
/// </summary>
public class CryptoService
{
    /// <summary>
    /// Encrypts the provided data using AES encryption and a shared secret.
    /// </summary>
    /// <typeparam name="T">The type of data to encrypt.</typeparam>
    /// <param name="data">The data to encrypt.</param>
    /// <param name="sharedSecret">The shared secret (encryption key).</param>
    /// <returns>The encrypted data as a byte array.</returns>
    public static async Task<byte[]> EncryptData<T>(T data, byte[] sharedSecret) where T : class
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = sharedSecret;

        using MemoryStream memoryStream = new MemoryStream();
        using (ICryptoTransform encryptor = aesAlg.CreateEncryptor())
        using (CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write))
        {
            await MemoryPackSerializer.SerializeAsync(cryptoStream, data);
        }

        // Return the encrypted data as a byte array
        return memoryStream.ToArray();
    }

    /// <summary>
    /// Decrypts the provided encrypted data using AES decryption and a shared secret.
    /// </summary>
    /// <typeparam name="T">The type of data to decrypt.</typeparam>
    /// <param name="encryptedData">The encrypted data to decrypt.</param>
    /// <param name="sharedSecret">The shared secret (encryption key).</param>
    /// <returns>The decrypted data of type T.</returns>
    public static async Task<T> DecryptData<T>(byte[] encryptedData, byte[] sharedSecret) where T : class
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = sharedSecret;

        using MemoryStream memoryStream = new MemoryStream(encryptedData);
        using ICryptoTransform decryptor = aesAlg.CreateDecryptor();
        using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        T decryptedData = await MemoryPackSerializer.DeserializeAsync<T>(cryptoStream);
        return decryptedData;
    }

    /// <summary>
    /// Computes an HMAC-SHA256 MAC (message authentication code) using a shared key.
    /// </summary>
    /// <param name="sharedKey">The shared key for computing the MAC.</param>
    /// <param name="data">The data for which to compute the MAC.</param>
    /// <returns>The computed HMAC-SHA256 MAC as a byte array.</returns>
    public static byte[] ComputeHmacSha256(byte[] sharedKey, byte[] data)
    {
        using HMACSHA256 hmac = new(sharedKey);
        return hmac.ComputeHash(data);
    }

    /// <summary>
    /// Compares a received MAC with a computed MAC to verify data integrity.
    /// </summary>
    /// <param name="sharedKey">The shared key used for MAC computation.</param>
    /// <param name="receivedMac">The received MAC to compare.</param>
    /// <param name="receivedData">The data for which the MAC was received.</param>
    /// <returns>True if the received MAC matches the computed MAC; otherwise, false.</returns>
    public static bool CompareMac(byte[] sharedKey, byte[] receivedMac, byte[] receivedData)
    {
        byte[] computedMac = ComputeHmacSha256(sharedKey, receivedData);
        return StructuralComparisons.StructuralEqualityComparer.Equals(computedMac, receivedMac);
    }
}


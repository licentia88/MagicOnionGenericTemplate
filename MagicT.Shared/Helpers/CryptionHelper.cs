using MemoryPack;
using System.Security.Cryptography;
using System.Collections;
using MagicT.Shared.Models.ServiceModels;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace MagicT.Shared.Helpers;

/// <summary>
/// Provides cryptographic operations for data encryption, decryption, and MAC computation.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class CryptionHelper
{
    /// <summary>
    /// Generates a random nonce of the specified size.
    /// </summary>
    /// <param name="size">The size of the nonce in bytes.</param>
    /// <returns>The generated nonce as a byte array.</returns>
    private static byte[] GenerateRandomNonce(int size)
    {
        byte[] nonce = new byte[size];

        using RandomNumberGenerator rng = RandomNumberGenerator.Create();

        rng.GetBytes(nonce);

        return nonce;
    }

    /// <summary>
    /// Encrypts the provided data using AES-GCM encryption and a shared secret.
    /// </summary>
    /// <typeparam name="TModel">The type of data to encrypt.</typeparam>
    /// <param name="data">The data to encrypt.</param>
    /// <param name="sharedSecret">The shared secret (encryption key).</param>
    /// <returns>The encrypted data as a byte array.</returns>
    public static async Task<EncryptedData<TModel>> EncryptData<TModel>(TModel data, byte[] sharedSecret)
    {
        SecureRandom random = new SecureRandom();

        // Generate a random nonce
        byte[] nonce = new byte[12];
        random.NextBytes(nonce);

        // Serialize the data
        using MemoryStream memoryStream = new();
        await MemoryPackSerializer.SerializeAsync(memoryStream, data);
        var serializedData = memoryStream.ToArray();

        // Encrypt the data using AES-GCM
        GcmBlockCipher cipher = new GcmBlockCipher(new AesEngine());
        cipher.Init(true, new AeadParameters(new KeyParameter(sharedSecret), 128, nonce));
        byte[] encryptedData = new byte[cipher.GetOutputSize(serializedData.Length)];
        int len = cipher.ProcessBytes(serializedData, 0, serializedData.Length, encryptedData, 0);
        cipher.DoFinal(encryptedData, len);

        return new EncryptedData<TModel>(encryptedData, nonce, cipher.GetMac());
    }

    /// <summary>
    /// Decrypts the provided encrypted data using AES-GCM decryption and a shared secret.
    /// </summary>
    /// <typeparam name="TModel">The type of data to decrypt.</typeparam>
    /// <param name="encryptedData">The encrypted data to decrypt.</param>
    /// <param name="sharedSecret">The shared secret (encryption key).</param>
    /// <returns>The decrypted data of type T.</returns>
    public static async Task<TModel> DecryptData<TModel>(EncryptedData<TModel> encryptedData, byte[] sharedSecret)  
    {
        using MemoryStream memoryStream = new MemoryStream();

        // Decrypt the data using AES-GCM
        GcmBlockCipher cipher = new GcmBlockCipher(new AesEngine());
        cipher.Init(false, new AeadParameters(new KeyParameter(sharedSecret), 128, encryptedData.Nonce));
        byte[] decryptedData = new byte[cipher.GetOutputSize(encryptedData.EncryptedBytes.Length)];
        int len = cipher.ProcessBytes(encryptedData.EncryptedBytes, 0, encryptedData.EncryptedBytes.Length, decryptedData, 0);
        cipher.DoFinal(decryptedData, len);

        // Deserialize the decrypted data
        memoryStream.Write(decryptedData, 0, decryptedData.Length);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return await MemoryPackSerializer.DeserializeAsync<TModel>(memoryStream);
    }


    /// <summary>
    /// Computes an HMAC-SHA256 MAC (message authentication code) using a shared key.
    /// </summary>
    /// <param name="sharedKey">The shared key for computing the MAC.</param>
    /// <param name="data">The data for which to compute the MAC.</param>
    /// <returns>The computed HMAC-SHA256 MAC as a byte array.</returns>
    [Obsolete(message: "Not needed anymore since we are using aes gcm")]
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
    [Obsolete(message: "Not needed anymore since we are using aes gcm")]
    public static bool CompareMac(byte[] sharedKey, byte[] receivedMac, byte[] receivedData)
    {
        byte[] computedMac = ComputeHmacSha256(sharedKey, receivedData);
        return StructuralComparisons.StructuralEqualityComparer.Equals(computedMac, receivedMac);
    }

}

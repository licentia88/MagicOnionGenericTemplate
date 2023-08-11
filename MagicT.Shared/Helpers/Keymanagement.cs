using System.Security.Cryptography;

namespace MagicT.Shared.Helpers;

/// <summary>
/// Provides key management operations, including key derivation and encryption using derived keys.
/// </summary>
[Obsolete(message: "Not needed anymore since we are using aes gcm")]
// ReSharper disable once UnusedType.Global
public sealed class KeyManagementService
{
    /// <summary>
    /// Derives an encryption key using PBKDF2 with a user secret and salt.
    /// </summary>
    /// <param name="userSecret">The user's secret (e.g., password) for key derivation.</param>
    /// <param name="salt">The salt value used for key derivation.</param>
    /// <returns>The derived encryption key as a byte array.</returns>
    public static byte[] DeriveEncryptionKey(string userSecret, byte[] salt)
    {
        using Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(userSecret, salt, 100000, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(32); // 256 bits for AES-256 encryption
    }

    

    /// <summary>
    /// Encrypts data using AES encryption and a derived key.
    /// </summary>
    /// <param name="data">The data to encrypt.</param>
    /// <param name="derivedKey">The derived encryption key.</param>
    /// <returns>The encrypted data as a byte array.</returns>
    public static byte[] EncryptUsingDerivedKey(byte[] data, byte[] derivedKey)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = derivedKey;
        aesAlg.GenerateIV();

        using MemoryStream msEncrypt = new MemoryStream();
        msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

        using CryptoStream csEncrypt = new(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write);
        csEncrypt.Write(data, 0, data.Length);
        csEncrypt.FlushFinalBlock();

        return msEncrypt.ToArray();
    }

    /// <summary>
    /// Decrypts data using AES decryption and a derived key.
    /// </summary>
    /// <param name="encryptedData">The encrypted data to decrypt.</param>
    /// <param name="derivedKey">The derived encryption key.</param>
    /// <returns>The decrypted data as a byte array.</returns>
    public static byte[] DecryptUsingDerivedKey(byte[] encryptedData, byte[] derivedKey)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = derivedKey;

        using MemoryStream msDecrypt = new(encryptedData);
        byte[] iv = new byte[aesAlg.IV.Length];
        var read = msDecrypt.Read(iv, 0, iv.Length);
        aesAlg.IV = iv;

        using CryptoStream csDecrypt = new(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read);
        using MemoryStream msDecrypted = new();
        csDecrypt.CopyTo(msDecrypted);
        return msDecrypted.ToArray();
    }
}


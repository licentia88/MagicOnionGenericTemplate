﻿using System.Security.Cryptography;
using MagicT.Shared.Models.ServiceModels;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace MagicT.Shared.Cryptography;

/// <summary>
/// Provides cryptographic operations for data encryption, decryption, and MAC computation.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class CryptoHelper
{
    // Thread-local storage for cipher reuse (if you know calls are thread-confined)
    private static readonly ThreadLocal<GcmBlockCipher> CipherCache = new(() => new GcmBlockCipher(new AesEngine()));
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

        return new EncryptedData<TModel>(encryptedMetadata.encryptedBytes, encryptedMetadata.nonce,
            encryptedMetadata.mac);
    }
/// <summary>
/// Encrypts the provided data using AES-GCM encryption and a shared secret.
/// </summary>
/// <typeparam name="TModel">The type of the data to encrypt.</typeparam>
/// <param name="data">The data to encrypt. Must not be null.</param>
/// <param name="sharedSecret">
/// The shared secret (encryption key). 
/// Must be a securely generated byte array of sufficient length (e.g., 16, 24, or 32 bytes for AES-128, AES-192, or AES-256).
/// </param>
/// <returns>
/// A tuple containing:
/// <list type="bullet">
/// <item><description><c>encryptedBytes</c>: The encrypted data as a byte array.</description></item>
/// <item><description><c>nonce</c>: A randomly generated nonce used during encryption (12 bytes).</description></item>
/// <item><description><c>mac</c>: The Message Authentication Code (MAC) ensuring data integrity and authenticity.</description></item>
/// </list>
/// </returns>
/// <exception cref="ArgumentNullException">
/// Thrown if <paramref name="sharedSecret"/> is null.
/// </exception>
/// <exception cref="ArgumentException">
/// Thrown if <paramref name="sharedSecret"/> does not meet the required length for AES encryption.
/// </exception>
public static (byte[] encryptedBytes, byte[] nonce, byte[] mac) EncryptWithMetaData<TModel>(TModel data, byte[] sharedSecret)
{
    if (sharedSecret == null)
        throw new ArgumentNullException(nameof(sharedSecret), "Shared secret cannot be null.");

    if (sharedSecret.Length != 16 && sharedSecret.Length != 24 && sharedSecret.Length != 32)
        throw new ArgumentException("Shared secret must be 16, 24, or 32 bytes long.", nameof(sharedSecret));

    var cipher = CipherCache.Value; // Reuse the cipher
    // cipher.Reset(); // Critical: Reset state before reuse    

    try
    {
        byte[] nonce = new byte[12];
        RandomNumberGenerator.Fill(nonce);

        byte[] serializedData = MemoryPackSerializer.Serialize(data);
        AeadParameters parameters = new AeadParameters(new KeyParameter(sharedSecret), 128, nonce);
        cipher.Init(true, parameters);

        byte[] encryptedBytes = new byte[cipher.GetOutputSize(serializedData.Length)];
    
        int len = cipher.ProcessBytes(serializedData, 0, serializedData.Length, encryptedBytes, 0);
        cipher.DoFinal(encryptedBytes, len);

        return (encryptedBytes, nonce, cipher.GetMac());
    }
    finally
    {
        cipher.Reset();

    }
    
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
        return DecryptWithMetaData<TModel>(encryptedData.EncryptedBytes, encryptedData.Nonce, encryptedData.Mac,
            sharedSecret);
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
    /// <exception cref="ArgumentException">Thrown if MAC verification fails.</exception>
    /// <exception cref="InvalidCipherTextException">Thrown if decryption fails due to invalid data or parameters.</exception>
    private static TModel DecryptWithMetaData<TModel>(byte[] encryptedData, byte[] nonce, byte[] mac, byte[] sharedSecret)
    {
        ArgumentNullException.ThrowIfNull(nonce);
        ArgumentNullException.ThrowIfNull(mac);
        ArgumentNullException.ThrowIfNull(sharedSecret);
        if (sharedSecret.Length != 16 && sharedSecret.Length != 24 && sharedSecret.Length != 32)
            throw new ArgumentException("Shared secret must be 16, 24, or 32 bytes long.", nameof(sharedSecret));

        var cipher = CipherCache.Value; // Reuse the cipher

        try
        {
 
            AeadParameters parameters = new AeadParameters(new KeyParameter(sharedSecret), 128, nonce);
            cipher.Init(false, parameters);

            byte[] decryptedData = new byte[cipher.GetOutputSize(encryptedData.Length)];
            int len = cipher.ProcessBytes(encryptedData, 0, encryptedData.Length, decryptedData, 0);
            cipher.DoFinal(decryptedData, len);

            // CHANGED: Using constant-time MAC comparison
            if (!CryptographicOperations.FixedTimeEquals(mac, cipher.GetMac()))
            {
                throw new ArgumentException("MAC verification failed. Data may have been tampered with.");
            }

            return MemoryPackSerializer.Deserialize<TModel>(decryptedData);
        }
        finally
        {
            cipher.Reset();
        }
    }
}


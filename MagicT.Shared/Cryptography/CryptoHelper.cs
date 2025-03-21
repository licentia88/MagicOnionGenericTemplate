﻿using System.Collections;
using MagicT.Shared.Models.ServiceModels;
using Org.BouncyCastle.Crypto;
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
    /// Thrown if <paramref name="data"/> or <paramref name="sharedSecret"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="sharedSecret"/> does not meet the required length for AES encryption.
    /// </exception>
    public static (byte[] encryptedBytes, byte[] nonce, byte[] mac) EncryptWithMetaData<TModel>(TModel data, byte[] sharedSecret)
    {
        // if (data == null)
        //     throw new ArgumentNullException(nameof(data), "Data to encrypt cannot be null.");

        if (sharedSecret == null)
            throw new ArgumentNullException(nameof(sharedSecret), "Shared secret cannot be null.");

        if (sharedSecret.Length != 16 && sharedSecret.Length != 24 && sharedSecret.Length != 32)
            throw new ArgumentException("Shared secret must be 16, 24, or 32 bytes long.", nameof(sharedSecret));

        // Create a new instance of GcmBlockCipher for thread safety
        GcmBlockCipher cipher = new(new AesEngine());
        
        SecureRandom random = new SecureRandom();

        // Generate a random nonce
        byte[] nonce = new byte[12];
        random.NextBytes(nonce);

        // Serialize the data
        byte[] serializedData = MemoryPackSerializer.Serialize(data);

        // Initialize cipher with parameters
        AeadParameters parameters = new AeadParameters(new KeyParameter(sharedSecret), 128, nonce);
        cipher.Init(true, parameters);

        // Encrypt data
        byte[] encryptedBytes = new byte[cipher.GetOutputSize(serializedData.Length)];
        int len = cipher.ProcessBytes(serializedData, 0, serializedData.Length, encryptedBytes, 0);
        cipher.DoFinal(encryptedBytes, len);

        // Return encrypted bytes, nonce, and MAC
        return (encryptedBytes, nonce, cipher.GetMac());
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
    public static TModel DecryptWithMetaData<TModel>(byte[] encryptedData, byte[] nonce, byte[] mac,
        byte[] sharedSecret)
    {
        // Validate input parameters
        // if (encryptedData == null) throw new ArgumentNullException(nameof(encryptedData));
        if (nonce == null) throw new ArgumentNullException(nameof(nonce));
        if (mac == null) throw new ArgumentNullException(nameof(mac));
        if (sharedSecret == null) throw new ArgumentNullException(nameof(sharedSecret));
        if (sharedSecret.Length != 16 && sharedSecret.Length != 24 && sharedSecret.Length != 32)
            throw new ArgumentException("Shared secret must be 16, 24, or 32 bytes long.", nameof(sharedSecret));

        // Create a new GcmBlockCipher instance for thread safety
        GcmBlockCipher cipher = new(new AesEngine());

        // Initialize the cipher with decryption parameters
        AeadParameters parameters = new AeadParameters(new KeyParameter(sharedSecret), 128, nonce);
        cipher.Init(false, parameters);

        // Decrypt the data
        byte[] decryptedData = new byte[cipher.GetOutputSize(encryptedData.Length)];
        int len = cipher.ProcessBytes(encryptedData, 0, encryptedData.Length, decryptedData, 0);
        cipher.DoFinal(decryptedData, len);

        // Validate the MAC after decryption
        if (!StructuralComparisons.StructuralEqualityComparer.Equals(mac, cipher.GetMac()))
        {
            throw new ArgumentException("MAC verification failed. Data may have been tampered with.");
        }

        // Deserialize the decrypted data
        return MemoryPackSerializer.Deserialize<TModel>(decryptedData);
    }
}


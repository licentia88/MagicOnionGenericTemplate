using MagicT.Shared.Models.ServiceModels;
using Org.BouncyCastle.Crypto;

namespace MagicT.Shared.Managers;

/// <summary>
/// Interface for managing key exchange operations.
/// </summary>
public interface IKeyExchangeManager
{
    /// <summary>
    /// Gets or sets the key exchange data.
    /// </summary>
    KeyExchangeData KeyExchangeData { get; set; }

    /// <summary>
    /// Creates a public key and returns the public key bytes and the private key.
    /// </summary>
    /// <returns>A tuple containing the public key bytes and the private key.</returns>
    (byte[] PublicBytes, AsymmetricKeyParameter PrivateKey) CreatePublicKey();

    /// <summary>
    /// Creates a shared key using the provided public key bytes and private key.
    /// </summary>
    /// <param name="publicBytes">The public key bytes.</param>
    /// <param name="privateKey">The private key.</param>
    /// <returns>The shared key as a byte array.</returns>
    byte[] CreateSharedKey(byte[] publicBytes, AsymmetricKeyParameter privateKey);
}
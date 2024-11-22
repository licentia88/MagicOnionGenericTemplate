using Benutomo;
using MagicT.Shared.Models.ServiceModels;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace MagicT.Shared.Managers;

/// <summary>
/// Manages key exchange operations using ECDH (Elliptic Curve Diffie-Hellman).
/// </summary>
[RegisterSingleton]
[AutomaticDisposeImpl]
public partial class KeyExchangeManager : IKeyExchangeManager, IDisposable
{
    /// <summary>
    /// Gets or sets the key exchange data.
    /// </summary>
    public KeyExchangeData KeyExchangeData { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyExchangeManager"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public KeyExchangeManager(IServiceProvider provider)
    {
        KeyExchangeData = provider.GetService<KeyExchangeData>();

        var ppKeyValuePair = CreatePublicKey();

        KeyExchangeData.SelfPublicBytes = ppKeyValuePair.PublicBytes;
        KeyExchangeData.PrivateKey = ppKeyValuePair.PrivateKey;
    }
    
    ~KeyExchangeManager()
    {
        Dispose(false);
    }

    /// <summary>
    /// Creates a public key and returns the public key bytes and the private key.
    /// </summary>
    /// <returns>A tuple containing the public key bytes and the private key.</returns>
    public (byte[] PublicBytes, AsymmetricKeyParameter PrivateKey) CreatePublicKey()
    {
        var ecParams = GetEcDomainParameters();
        var keyPair = GenerateKeyPair(ecParams);

        var publicKeyBytes = GetPublicKeyBytes(keyPair.Public as ECPublicKeyParameters);

        return (publicKeyBytes, keyPair.Private);
    }

    /// <summary>
    /// Creates a shared key using the provided public key bytes and private key.
    /// </summary>
    /// <param name="publicBytes">The public key bytes.</param>
    /// <param name="privateKey">The private key.</param>
    /// <returns>The shared key as a byte array.</returns>
    public byte[] CreateSharedKey(byte[] publicBytes, AsymmetricKeyParameter privateKey)
    {
        var ecParams = GetEcDomainParameters();
        var receivedPublicKey = new ECPublicKeyParameters(ecParams.Curve.DecodePoint(publicBytes), ecParams);

        return PerformKeyAgreement(receivedPublicKey, privateKey);
    }

   /// <summary>
/// Gets the elliptic curve domain parameters for the P-256 curve.
/// </summary>
/// <returns>The elliptic curve domain parameters.</returns>
private static ECDomainParameters GetEcDomainParameters()
{
    var ecP = ECNamedCurveTable.GetByName("P-256");
    return new ECDomainParameters(ecP.Curve, ecP.G, ecP.N, ecP.H);
}

/// <summary>
/// Generates an asymmetric key pair using the specified elliptic curve domain parameters.
/// </summary>
/// <param name="ecParams">The elliptic curve domain parameters.</param>
/// <returns>The generated asymmetric key pair.</returns>
private static AsymmetricCipherKeyPair GenerateKeyPair(ECDomainParameters ecParams)
{
    var keyGen = new ECKeyPairGenerator();
    keyGen.Init(new ECKeyGenerationParameters(ecParams, new SecureRandom()));
    return keyGen.GenerateKeyPair();
}

/// <summary>
/// Gets the public key bytes from the specified public key parameters.
/// </summary>
/// <param name="publicKey">The public key parameters.</param>
/// <returns>The public key bytes.</returns>
private static byte[] GetPublicKeyBytes(ECPublicKeyParameters publicKey)
{
    return publicKey?.Q.GetEncoded();
}

/// <summary>
/// Performs key agreement using the specified public key and private key.
/// </summary>
/// <param name="receivedPublicKey">The received public key parameters.</param>
/// <param name="privateKey">The private key parameters.</param>
/// <returns>The shared secret as a byte array.</returns>
private static byte[] PerformKeyAgreement(ECPublicKeyParameters receivedPublicKey, AsymmetricKeyParameter privateKey)
{
    var agreement = new ECDHBasicAgreement();
    agreement.Init(privateKey);
    var sharedSecret = agreement.CalculateAgreement(receivedPublicKey);
    return sharedSecret.ToByteArrayUnsigned();
}
}
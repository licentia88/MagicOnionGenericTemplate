using MagicT.Shared.Models.ServiceModels;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace MagicT.Shared.Managers;

public class KeyExchangeManager : IKeyExchangeManager
{
    public KeyExchangeData KeyExchangeData { get; set; }

    public KeyExchangeManager(IServiceProvider provider)
    {
        KeyExchangeData = provider.GetService<KeyExchangeData>();
    }

    /// <summary>
    /// Generates a new ECDH public key and returns the raw bytes.
    /// </summary>
    /// <returns>The public key as a byte array</returns>
    public  (byte[] PublicBytes, AsymmetricKeyParameter PrivateKey) CreatePublicKey()
    {
        // Create EC domain parameters using named curve
        X9ECParameters ecP = ECNamedCurveTable.GetByName("P-256");
        ECDomainParameters ecParams = new ECDomainParameters(ecP.Curve, ecP.G, ecP.N, ecP.H);

        // Generate new EC key pair 
        SecureRandom random = new SecureRandom();
        ECKeyPairGenerator keyGen = new ECKeyPairGenerator();
        keyGen.Init(new ECKeyGenerationParameters(ecParams, random));
        AsymmetricCipherKeyPair serverKeyPair = keyGen.GenerateKeyPair();

        // Extract public key from pair
        ECPublicKeyParameters serverPubKey = serverKeyPair.Public as ECPublicKeyParameters;

        // Encode public key to raw bytes
        byte[] publicKeyBytes = serverPubKey.Q.GetEncoded();

        // Return the raw public key bytes
        return (publicKeyBytes, serverKeyPair.Private);
    }

    /// <summary>
    /// Computes ECDH shared secret using the received public key.
    /// </summary>
    /// <param name="publicKeyBytes">The public key from server</param>
    /// <returns>Shared secret as byte array</returns>
    public byte[] CreateSharedKey(byte[] PublicBytes, AsymmetricKeyParameter PrivateKey)
    {
        X9ECParameters ecP = ECNamedCurveTable.GetByName("P-256");
        ECDomainParameters ecParams = new ECDomainParameters(ecP.Curve, ecP.G, ecP.N, ecP.H);

        // Convert received public key bytes to an ECPublicKeyParameters
        ECPublicKeyParameters receivedPublicKey = new ECPublicKeyParameters(
            ecParams.Curve.DecodePoint(PublicBytes), ecParams);

        // Perform key agreement
        ECDHBasicAgreement agreement = new ECDHBasicAgreement();
        agreement.Init(PrivateKey);
        BigInteger sharedSecret = agreement.CalculateAgreement(receivedPublicKey);

        // Convert shared secret to byte array
        byte[] sharedSecretBytes = sharedSecret.ToByteArrayUnsigned();

        return sharedSecretBytes;
    }

    public void Initialize()
    {
        var PPKeyValuePair = CreatePublicKey();

        KeyExchangeData.SelfPublicBytes = PPKeyValuePair.PublicBytes;

        KeyExchangeData.PrivateKey = PPKeyValuePair.PrivateKey;
    }
}

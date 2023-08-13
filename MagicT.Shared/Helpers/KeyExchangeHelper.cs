using System.Security.Cryptography;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace MagicT.Shared.Helpers;

public static class KeyExchangeHelper
{

    public static byte[] Method()
    {

        // Generate key pairs
        X9ECParameters ecP = ECNamedCurveTable.GetByName("P-256");
        ECDomainParameters ecParams = new ECDomainParameters(ecP.Curve, ecP.G, ecP.N, ecP.H);
        ECKeyPairGenerator keyGen = new ECKeyPairGenerator();
        keyGen.Init(new ECKeyGenerationParameters(ecParams, new SecureRandom()));

        AsymmetricCipherKeyPair aliceKeyPair = keyGen.GenerateKeyPair();
        AsymmetricCipherKeyPair bobKeyPair = keyGen.GenerateKeyPair();

        // Extract public keys
        ECPublicKeyParameters alicePubKey = aliceKeyPair.Public as ECPublicKeyParameters;
        ECPublicKeyParameters bobPubKey = bobKeyPair.Public as ECPublicKeyParameters;


        // ECDH key agreement 
        ECDHBasicAgreement agree = new ECDHBasicAgreement();
        BigInteger secret = agree.CalculateAgreement(bobPubKey);

 
        return secret.ToByteArray();
    }
    public static byte[] CreatePublicKey(this ECDiffieHellmanCng edh)
    {
        return edh.ExportSubjectPublicKeyInfo();
    }

    public static byte[] CreateSharedKey(this ECDiffieHellmanCng edh, byte[] publicKey)
    {
        return edh.DeriveKeyMaterial(CngKey.Import(publicKey, CngKeyBlobFormat.EccPublicBlob));
    }
}

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
// ReSharper disable UnusedType.Global

namespace MagicT.Server.Helpers;
 
/// <summary>
/// Provides helper methods for working with X509 certificates.
/// </summary>
public static class CertificateHelper
{
    /// <summary>
    /// Gets an X509 certificate from the specified CRT and key file paths.
    /// </summary>
    /// <param name="pathToCrt">Path to the certificate (CRT) file.</param>
    /// <param name="pathToKey">Path to the private key file.</param>
    /// <returns>An X509 certificate.</returns>
    public static X509Certificate2 GetCertificate(string pathToCrt, string pathToKey)
    {
        // Alternatively, if you have a .pem file, you can use the CreateFromPublicPrivateKey method
        var certificate = CreateFromPemFile(pathToCrt, pathToKey);

        // On Windows, there is a bug that requires converting to PKCS12 format
        if (PlatFormHelper.IsWindows())
            return new X509Certificate2(certificate.Export(X509ContentType.Pkcs12));

        return certificate;
    }

    /// <summary>
    /// Creates an X509 certificate from PEM-encoded CRT and private key files.
    /// </summary>
    /// <param name="pathToCrt">Path to the certificate (CRT) file.</param>
    /// <param name="pathToKey">Path to the private key file.</param>
    /// <returns>An X509 certificate.</returns>
    public static X509Certificate2 CreateFromPemFile(string pathToCrt, string pathToKey)
    {
        return X509Certificate2.CreateFromPemFile(pathToCrt, pathToKey);
    }

    /// <summary>
    /// Creates an X509 certificate from PEM-encoded public and private key strings.
    /// </summary>
    /// <param name="publicPem">PEM-encoded public key.</param>
    /// <param name="privatePem">PEM-encoded private key.</param>
    /// <returns>An X509 certificate with a private key.</returns>
    public static X509Certificate2 CreateFromPublicPrivateKey(string publicPem, string privatePem)
    {
        byte[] publicPemBytes = File.ReadAllBytes(publicPem);
        using var publicX509 = new X509Certificate2(publicPemBytes);

        var privateKeyText = File.ReadAllText(privatePem);
        var privateKeyBlocks = privateKeyText.Split("-", StringSplitOptions.RemoveEmptyEntries);
        var privateKeyBytes = Convert.FromBase64String(privateKeyBlocks[1]);

        using RSA rsa = RSA.Create();
        switch (privateKeyBlocks[0])
        {
            case "BEGIN PRIVATE KEY":
                rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
                break;
            case "BEGIN RSA PRIVATE KEY":
                rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
                break;
        }
        X509Certificate2 keyPair = publicX509.CopyWithPrivateKey(rsa);
        return keyPair;
    }
}
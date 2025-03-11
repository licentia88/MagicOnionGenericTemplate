using Org.BouncyCastle.Security;

namespace MagicT.Redis.Security;

/// <summary>
///  Provides a method for generating a random AES key and IV.
/// </summary>
public static class AesKeyGenerator
{
    /// <summary>
    ///  Generates a random AES key and IV.
    /// </summary>
    /// <returns></returns>
    public static (byte[] Key, byte[] IV) GenerateKeyAndIv()
    {
        var key = new byte[32]; // 256-bit key
        var iv = new byte[16];  // 128-bit IV

        new SecureRandom().NextBytes(key);
        new SecureRandom().NextBytes(iv);

        return (key, iv);
    }
}
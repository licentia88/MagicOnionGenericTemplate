using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace MagicT.Redis.Security;

public static class MagicTSecurity
{
    // Encrypts a byte array using AES (CBC mode, PKCS7 padding)
    public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
    {
        var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()));
        cipher.Init(true, new ParametersWithIV(new KeyParameter(key), iv));

        return ProcessCipher(cipher, data);
    }

    // Decrypts a byte array using AES (CBC mode, PKCS7 padding)
    public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
    {
        var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()));
        cipher.Init(false, new ParametersWithIV(new KeyParameter(key), iv));

        return ProcessCipher(cipher, data);
    }

    // Helper method to process the cipher (both for encryption and decryption)
    private static byte[] ProcessCipher(IBufferedCipher cipher, byte[] input)
    {
        byte[] output = new byte[cipher.GetOutputSize(input.Length)];
        int len = cipher.ProcessBytes(input, 0, input.Length, output, 0);
        len += cipher.DoFinal(output, len);
        Array.Resize(ref output, len);
        return output;
    }
}
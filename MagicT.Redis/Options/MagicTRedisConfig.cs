using MagicT.Redis.Security;

namespace MagicT.Redis.Options;

public class MagicTRedisConfig : IDisposable
{
    /// <summary>
    /// The Redis connection string used to connect to the Redis server.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// The encryption key used for encrypting and decrypting data stored in Redis.
    /// </summary>
    public byte[] EncryptionKey { get; set; }

    /// <summary>
    /// The initialization vector (IV) used for encrypting and decrypting data stored in Redis.
    /// </summary>
    public byte[] EncryptionIv { get; set; }

 
    /// <summary>
    /// Initializes a new instance of the <see cref="MagicTRedisConfig"/> class.
    /// </summary>
    public MagicTRedisConfig()
    {
         
    }

    /// <summary>
    /// Finalizer to ensure resources are cleaned up if Dispose is not called.
    /// </summary>
    ~MagicTRedisConfig()
    {
        Dispose(false);
    }

    /// <summary>
    /// Disposes of the resources used by the <see cref="MagicTRedisConfig"/> class.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Cleans up resources used by the <see cref="MagicTRedisConfig"/> class.
    /// </summary>
    /// <param name="disposing">True if called from Dispose, false if called from the finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        // Clear sensitive data from memory
        if (EncryptionKey != null)
        {
            Array.Clear(EncryptionKey, 0, EncryptionKey.Length);
            EncryptionKey = null;
        }

        if (EncryptionIv != null)
        {
            Array.Clear(EncryptionIv, 0, EncryptionIv.Length);
            EncryptionIv = null;
        }
    }
}
namespace MagicT.Client.Models;

/// <summary>
/// Represents the credentials of a MagicT user.
/// </summary>
public sealed class MagicTUserCredentials
{
    /// <summary>
    /// Gets or sets the shared key of the user.
    /// </summary>
    public byte[] SharedKey { get; set; }

    /// <summary>
    /// Gets or sets the token of the user.
    /// </summary>
    public byte[] Token { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MagicTUserCredentials"/> class.
    /// </summary>
    /// <param name="sharedKey"></param>
    /// <param name="token"></param>
    public MagicTUserCredentials(byte[] sharedKey, byte[] token)
    {
        SharedKey = sharedKey;
        Token = token;
    }
     
}

 
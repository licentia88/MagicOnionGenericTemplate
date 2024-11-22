using Benutomo;
using Grpc.Core;
using LitJWT;
using MagicOnion;
using MagicT.Server.Jwt;
using Utf8Json;

namespace MagicT.Server.Managers;

/// <summary>
/// Manages token operations including processing JWT tokens.
/// </summary>
[RegisterSingleton]
[AutomaticDisposeImpl]
public partial class TokenManager : IDisposable
{
    /// <summary>
    /// Gets or initializes the JWT encoder used for creating tokens.
    /// </summary>
    public JwtEncoder Encoder { get; }

    /// <summary>
    /// Gets or initializes the JWT decoder used for decoding tokens.
    /// </summary>
    public JwtDecoder Decoder { get; }

    public TokenManager(IServiceProvider provider)
    {
        Encoder = provider.GetService<JwtEncoder>();
        Decoder = provider.GetService<JwtDecoder>();
    }
    
    ~TokenManager()
    {
        Dispose(false);
    }
    /// <summary>
    /// Creates a JWT token with the specified contact identifier and roles.
    /// </summary>
    /// <param name="contactIdentifier">The contact identifier (email or phone number) for whom the token is being created.</param>
    /// <param name="identifier"></param>
    /// <param name="roles">An array of role IDs associated with the user.</param>
    /// <param name="id"></param>
    /// <returns>A byte array containing the encoded JWT token.</returns>
    public byte[] CreateToken(int id, string identifier, params int[] roles)
    {
        // Encode a MagicTToken instance into a JWT token using the JwtEncoder.
        var token = Encoder.EncodeAsUtf8Bytes(new MagicTToken(id,identifier, roles), TimeSpan.FromDays(1),
            (x, writer) => writer.Write(JsonSerializer.SerializeUnsafe(x)));

        return token;
    }


    /// <summary>
    /// Decodes a JWT token and returns the associated MagicTToken.
    /// </summary>
    /// <param name="token">The byte array containing the JWT token to decode.</param>
    /// <returns>The decoded MagicTToken.</returns>
    /// <exception cref="ReturnStatusException">Thrown if token decoding fails.</exception>
    internal MagicTToken DecodeToken(byte[] token)
    { 
        // Attempt to decode the provided JWT token using the JwtDecoder.
        var result = Decoder.TryDecode(token, x => JsonSerializer.Deserialize<MagicTToken>(x.ToArray()), out var tokenResult);

        // Check the decoding result and handle errors.
        if (result != DecodeResult.Success)
            throw new ReturnStatusException(StatusCode.Unauthenticated, result.ToString());

        return tokenResult;
    }
}
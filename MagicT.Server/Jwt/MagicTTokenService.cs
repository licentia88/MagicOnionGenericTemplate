using Grpc.Core;
using LitJWT;
using MagicOnion;

namespace MagicT.Server.Jwt;

/// <summary>
/// Service responsible for creating and decoding JWT tokens for MagicT server.
/// </summary>
public sealed class MagicTTokenService
{
    /// <summary>
    /// Gets or initializes the JWT encoder used for creating tokens.
    /// </summary>
    public JwtEncoder Encoder { get; init; }

    /// <summary>
    /// Gets or initializes the JWT decoder used for decoding tokens.
    /// </summary>
    public JwtDecoder Decoder { get; init; }

    /// <summary>
    /// Creates a JWT token with the specified user ID and roles.
    /// </summary>
    /// <param name="userId">The ID of the user for whom the token is being created.</param>
    /// <param name="roles">An array of role IDs associated with the user.</param>
    /// <returns>A byte array containing the encoded JWT token.</returns>
    public byte[] CreateToken(int userId, params int[] roles)
    {
        // Encode a MagicTToken instance into a JWT token using the JwtEncoder.
        var token = Encoder.EncodeAsUtf8Bytes(new MagicTToken(userId, roles),
            TimeSpan.FromMinutes(1),
            (x, writer) => writer.Write(Utf8Json.JsonSerializer.SerializeUnsafe(x)));

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
        var result = Decoder.TryDecode(token, x => Utf8Json.JsonSerializer.Deserialize<MagicTToken>(x.ToArray()), out var TokenResult);

        // Check the decoding result and handle errors.
        if (result != DecodeResult.Success)
            throw new ReturnStatusException(StatusCode.Unauthenticated, result.ToString());

        return TokenResult;
    }
}
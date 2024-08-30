﻿using MagicOnion;

namespace MagicT.Server.Jwt;

/// <summary>
/// Service responsible for creating and decoding JWT tokens for MagicT server.
/// </summary>
[global::Benutomo.AutomaticDisposeImpl]
public  sealed partial class MagicTTokenService: global::System.IDisposable, global::System.IAsyncDisposable
{
    /// <summary>
    /// Gets or initializes the JWT encoder used for creating tokens.
    /// </summary>
    public global::LitJWT.JwtEncoder Encoder { get; init; }

    /// <summary>
    /// Gets or initializes the JWT decoder used for decoding tokens.
    /// </summary>
    public global::LitJWT.JwtDecoder Decoder { get; init; }

    /// <summary>
    /// Creates a JWT token with the specified contact identifier and roles.
    /// </summary>
    /// <param name="contactIdentifier">The contact identifier (email or phone number) for whom the token is being created.</param>
    /// <param name="roles">An array of role IDs associated with the user.</param>
    /// <returns>A byte array containing the encoded JWT token.</returns>
    public byte[] CreateToken(int Id, string identifier, params int[] roles)
    {
        // Encode a MagicTToken instance into a JWT token using the JwtEncoder.
        var token = Encoder.EncodeAsUtf8Bytes(new MagicTToken(Id,identifier, roles), global::System.TimeSpan.FromDays(1),
            (x, writer) => writer.Write(global::Utf8Json.JsonSerializer.SerializeUnsafe(x)));

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
        var result = Decoder.TryDecode(token, x => global::Utf8Json.JsonSerializer.Deserialize<MagicTToken>(x.ToArray()), out var TokenResult);

        // Check the decoding result and handle errors.
        if (result != global::LitJWT.DecodeResult.Success)
            throw new global::MagicOnion.ReturnStatusException(global::Grpc.Core.StatusCode.Unauthenticated, result.ToString());

        return TokenResult;
    }
}
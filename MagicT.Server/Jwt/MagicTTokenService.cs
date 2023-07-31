using Grpc.Core;
using LitJWT;
using MagicOnion;

namespace MagicT.Server.Jwt;

public class MagicTTokenService
{
    public JwtEncoder Encoder { get; set; }

    public JwtDecoder Decoder { get; set; }

    public byte[] CreateToken(int userId, params int[] roles) //where TModel :class
    {
        var token = Encoder.EncodeAsUtf8Bytes((userId, roles),
            TimeSpan.FromMinutes(1),
            (x, writer) => writer.Write(Utf8Json.JsonSerializer.SerializeUnsafe(x)));

        return token;
    }

    internal bool DecodeToken(byte[] token, int[] roles)
    {
        var result = Decoder.TryDecode(token, x => Utf8Json.JsonSerializer.Deserialize<(int userId, List<int> Roles)>(x.ToArray()), out var TokenResult);

        if (result != DecodeResult.Success)
            throw new ReturnStatusException(StatusCode.Cancelled, result.ToString());

        return true;
    }
}

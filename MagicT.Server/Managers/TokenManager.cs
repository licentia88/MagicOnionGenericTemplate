using Grpc.Core;
using MagicOnion;
using MagicT.Server.Jwt;

namespace MagicT.Server.Managers;

public class TokenManager
{
    private MagicTTokenService MagicTTokenService { get; set; }


    public TokenManager(IServiceProvider provider)
    {
        MagicTTokenService = provider.GetService<MagicTTokenService>();
    }

    /// <summary>
    /// Processes the JWT token from the request headers.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ReturnStatusException"></exception>
    public MagicTToken Process(byte[] token)
    {
        if (token is null)
            throw new ReturnStatusException(StatusCode.NotFound, "Security Token not found");

        return MagicTTokenService.DecodeToken(token);
    }
}

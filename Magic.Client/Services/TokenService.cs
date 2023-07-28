using Magic.Client.Services.Base;
using Magic.Shared.Services;
using MagicOnion;

namespace Magic.Client.Services;

[RegisterSingleton]
public class TokenService : ServiceBase<ITokenService, byte[]>, ITokenService
{
    public TokenService()
    {
    }

    public UnaryResult<byte[]> Request(int id, string password)
    {
        return Client.Request(id, password);
    }
}


using MagicOnion;
using MagicT.Client.Services.Base;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

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


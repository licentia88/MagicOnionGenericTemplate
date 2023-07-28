using Magic.Shared.Services.Base;
using MagicOnion;

namespace Magic.Shared.Services;

public interface ITokenService : IGenericService<ITokenService, byte[]>
{
    public UnaryResult<byte[]> Request(int id, string password);
}
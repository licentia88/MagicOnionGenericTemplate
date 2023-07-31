using MagicOnion;
using MagicT.Shared.Services.Base;

namespace MagicT.Shared.Services;

public interface ITokenService : IGenericService<ITokenService, byte[]>
{
    public UnaryResult<byte[]> Request(int id, string password);
}
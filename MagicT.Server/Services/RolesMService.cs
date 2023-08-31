using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public class RolesMService : MagicServerServiceBase<IRolesMService, ROLES_M>, IRolesMService
{
    public RolesMService(IServiceProvider provider) : base(provider)
    {
    }
}
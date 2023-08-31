using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public class RolesDService : MagicServerServiceBase<IRolesDService, ROLES_D>, IRolesDService
{
    public RolesDService(IServiceProvider provider) : base(provider)
    {
    }
}
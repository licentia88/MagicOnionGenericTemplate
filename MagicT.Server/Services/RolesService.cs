using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public class RolesService : MagicServerService<IRolesService, ROLES>, IRolesService
{
    public RolesService(IServiceProvider provider) : base(provider)
    {
    }
}

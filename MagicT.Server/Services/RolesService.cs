using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public class RolesService : MagicServerSecureService<IRolesService, ROLES, MagicTContext>, IRolesService
{
    public RolesService(IServiceProvider provider) : base(provider)
    {
    }
}

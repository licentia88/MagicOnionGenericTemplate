using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public class UserRolesService : MagicServerServiceBase<IUserRolesService, USER_ROLES>, IUserRolesService
{
    public UserRolesService(IServiceProvider provider) : base(provider)
    {
    }
}

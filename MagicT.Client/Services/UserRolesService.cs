using MagicOnion;
using MagicT.Client.Filters;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

public sealed class UserRolesService : MagicClientSecureService<IUserRolesService, USER_ROLES>, IUserRolesService
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    public UserRolesService(IServiceProvider provider) : base(provider)
    {
    }

    public UnaryResult<List<USER_ROLES>> FindUserRolesByType(string RoleType)
    {
        return Client.FindUserRolesByType(RoleType);
    }
}


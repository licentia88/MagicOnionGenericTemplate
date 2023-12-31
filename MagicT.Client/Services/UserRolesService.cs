using MagicOnion;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

[RegisterScoped]
public sealed class UserRolesService : MagicClientSecureService<IUserRolesService, USER_ROLES>, IUserRolesService
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    public UserRolesService(IServiceProvider provider) : base(provider)
    {
    }
 

    public UnaryResult<List<USER_ROLES>> FindUserRolesByType(int userId, string RoleType)
    {
        return Client.FindUserRolesByType(userId,RoleType);
    }
}


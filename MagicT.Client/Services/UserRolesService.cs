using Benutomo;
using MagicOnion;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

[RegisterScoped]
[AutomaticDisposeImpl]
public partial class UserRolesService : MagicClientSecureService<IUserRolesService, USER_ROLES>, IUserRolesService, IDisposable
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    public UserRolesService(IServiceProvider provider) : base(provider)
    {
    }
    
    ~UserRolesService()
    {
        Dispose(false);
    }
 

    public UnaryResult<List<USER_ROLES>> FindUserRolesByType(int userId, string RoleType)
    {
        return Client.FindUserRolesByType(userId,RoleType);
    }
}


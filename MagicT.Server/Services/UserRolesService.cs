using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace MagicT.Server.Services;

public class UserRolesService : AuthorizationSeviceBase<IUserRolesService, USER_ROLES,MagicTContext>, IUserRolesService
{
    public UserRolesService(IServiceProvider provider) : base(provider)
    {
    }

   

    public async UnaryResult<List<USER_ROLES>> FindUserRolesByType(string RoleType)
    {
        //IgnoreTransaction = true;
        return await ExecuteAsyncWithoutResponse(async () =>
        {
            return await Db.USER_ROLES
                       .Where(x => x.AUTHORIZATIONS_BASE.AB_AUTH_TYPE == RoleType)
                       .AsNoTracking().ToListAsync();
        });
    }
}

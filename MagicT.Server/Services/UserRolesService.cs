using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services;

public class UserRolesService : MagicServerServiceAuth<IUserRolesService, USER_ROLES, MagicTContextAudit>, IUserRolesService
{
    public UserRolesService(IServiceProvider provider) : base(provider)
    {

    }

    public async UnaryResult<List<USER_ROLES>> FindUserRolesByType(string RoleType)
    {
        //IgnoreTransaction = true;
        return await ExecuteWithoutResponseAsync(async () =>
        {
            return await Database.USER_ROLES
                       .Where(x => x.AUTHORIZATIONS_BASE.AB_AUTH_TYPE == RoleType)
                       .AsNoTracking().ToListAsync();
        });
    }
}

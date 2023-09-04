using MagicOnion;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services;

public class UserRolesService : MagicServerServiceBase<IUserRolesService, USER_ROLES>, IUserRolesService
{
    public UserRolesService(IServiceProvider provider) : base(provider)
    {
    }

    public override async UnaryResult<List<USER_ROLES>> FindByParent(string parentId, string foreignKey)
    {
        return await ExecuteAsyncWithoutResponse(async () =>
        {
            int.TryParse(parentId, out int userRefNo);
            return await Db.USER_ROLES
                        .Include(x => x.AUTHORIZATIONS_BASE)
                        .Where(x => x.UR_USER_REFNO == userRefNo)
                        .AsNoTracking().ToListAsync();
        });
    }

    public async UnaryResult<List<USER_ROLES>> FindUserRolesByType(string RoleType)
    {
        return await ExecuteAsyncWithoutResponse(async () =>
        {
            return await Db.USER_ROLES
                       .Include(x => x.AUTHORIZATIONS_BASE)
                       .Where(x => x.AUTHORIZATIONS_BASE.AB_AUTH_TYPE == RoleType)
                       .AsNoTracking().ToListAsync();
        });
    }
}

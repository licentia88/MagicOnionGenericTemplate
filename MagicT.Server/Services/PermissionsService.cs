using MagicOnion;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services;

public class PermissionsService : MagicServerServiceBase<IPermissionsService, PERMISSIONS>, IPermissionsService
{
    public PermissionsService(IServiceProvider provider) : base(provider)
    {
    }

    public override async UnaryResult<List<PERMISSIONS>> FindByParent(string parentId, string foreignKey)
    {
        return await ExecuteAsyncWithoutResponse(async () =>
        {
            int.TryParse(parentId, out int Id);

            var response = Db.PERMISSIONS.Where(x => x.PER_ROLE_REFNO == Id).AsNoTracking();

            return await response.ToListAsync();
        });
        //return base.FindByParent(parentId, foreignKey);
    }
}
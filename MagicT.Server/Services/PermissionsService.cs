using MagicOnion;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services;

public class PermissionsService : MagicServerServiceBase<IPermissionsService, PERMISSIONS>, IPermissionsService
{
    public Lazy<List<PERMISSIONS>> PermissionList { get; set; }

    public PermissionsService(IServiceProvider provider) : base(provider)
    {
        PermissionList = provider.GetService<Lazy<List<PERMISSIONS>>>();
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

    public override async UnaryResult<PERMISSIONS> Create(PERMISSIONS model)
    {
        var result = await base.Create(model);
        PermissionList.Value.Add(model);
        return result;
    }

   
    public override async UnaryResult<PERMISSIONS> Delete(PERMISSIONS model)
    {
        var result = await base.Delete(model);
        PermissionList.Value.Remove(model);
        return result;
    }

}
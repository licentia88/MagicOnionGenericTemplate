using Benutomo;
using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services;

[AutomaticDisposeImpl]
public partial class PermissionsService : MagicServerAuthService<IPermissionsService, PERMISSIONS,MagicTContext>, IPermissionsService, IDisposable,IAsyncDisposable
{
 
    public PermissionsService(IServiceProvider provider) : base(provider)
    {
        //PermissionList = provider.GetService<Lazy<List<PERMISSIONS>>>();
    }

    public override async UnaryResult<List<PERMISSIONS>> FindByParentAsync(string parentId, string foreignKey)
    {
        return await ExecuteAsync(async () =>
        {
            int.TryParse(parentId, out int Id);

            var response = Db.PERMISSIONS.Where(x => x.PER_ROLE_REFNO == Id).AsNoTracking();

            return await response.ToListAsync();
        });
        //return base.FindByParent(parentId, foreignKey);
    }

    public override async UnaryResult<PERMISSIONS> CreateAsync(PERMISSIONS model)
    {
        var result = await base.CreateAsync(model);

        MagicTRedisDatabase.Create(model.PER_PERMISSION_NAME, model);
        //PermissionList.Value.Add(model);
        return result;
    }

    public override UnaryResult<PERMISSIONS> UpdateAsync(PERMISSIONS model)
    {
        var result = base.UpdateAsync(model);

        MagicTRedisDatabase.Update(model.PER_PERMISSION_NAME, model);
        //PermissionList.Value.Add(model);
        return result;
    }


    public override async UnaryResult<PERMISSIONS> DeleteAsync(PERMISSIONS model)
    {
        var result = await base.DeleteAsync(model);
        MagicTRedisDatabase.Delete<PERMISSIONS>(model.PER_PERMISSION_NAME);
        return result;
    }

}
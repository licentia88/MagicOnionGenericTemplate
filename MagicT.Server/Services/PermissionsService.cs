using Benutomo;
using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services;

/// <summary>
/// Service for handling permission-related operations with encryption and authorization.
/// </summary>
[AutomaticDisposeImpl]
// ReSharper disable once UnusedType.Global
public partial class PermissionsService : MagicServerSecureService<IPermissionsService, PERMISSIONS, MagicTContext>, IPermissionsService, IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionsService"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public PermissionsService(IServiceProvider provider) : base(provider)
    {
        //PermissionList = provider.GetService<Lazy<List<PERMISSIONS>>>();
    }

    /// <summary>
    /// Finds permissions by parent ID and foreign key.
    /// </summary>
    /// <param name="parentId">The parent ID.</param>
    /// <param name="foreignKey">The foreign key.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing a list of <see cref="PERMISSIONS"/>.</returns>
    public override async UnaryResult<List<PERMISSIONS>> FindByParentAsync(string parentId, string foreignKey)
    {
        return await ExecuteAsync(async () =>
        {
            int.TryParse(parentId, out int id);

            var response = Db.PERMISSIONS.Where(x => x.PER_ROLE_REFNO == id).AsNoTracking();

            return await response.ToListAsync();
        });
        //return base.FindByParent(parentId, foreignKey);
    }

    /// <summary>
    /// Creates a new permission.
    /// </summary>
    /// <param name="model">The permission model.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the created <see cref="PERMISSIONS"/>.</returns>
    public override async UnaryResult<PERMISSIONS> CreateAsync(PERMISSIONS model)
    {
        var result = await base.CreateAsync(model);

        MagicTRedisDatabase.Create(model.PER_PERMISSION_NAME, model);
        //PermissionList.Value.Add(model);
        return result;
    }

    /// <summary>
    /// Updates an existing permission.
    /// </summary>
    /// <param name="model">The permission model.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the updated <see cref="PERMISSIONS"/>.</returns>
    public override async UnaryResult<PERMISSIONS> UpdateAsync(PERMISSIONS model)
    {
        var result = await base.UpdateAsync(model);

        MagicTRedisDatabase.Update(model.PER_PERMISSION_NAME, model);
        //PermissionList.Value.Add(model);
        return result;
    }

    /// <summary>
    /// Deletes an existing permission.
    /// </summary>
    /// <param name="model">The permission model.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the deleted <see cref="PERMISSIONS"/>.</returns>
    public override async UnaryResult<PERMISSIONS> DeleteAsync(PERMISSIONS model)
    {
        var result = await base.DeleteAsync(model);
        MagicTRedisDatabase.Delete<PERMISSIONS>(model.PER_PERMISSION_NAME);
        return result;
    }
}
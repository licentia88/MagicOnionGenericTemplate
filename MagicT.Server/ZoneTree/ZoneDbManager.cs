using MagicT.Server.Database;
using MagicT.Server.ZoneTree.Zones;
using MagicT.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.ZoneTree;

public class ZoneDbManager
{
    public UsedTokensZoneDb UsedTokensZoneDb { get; set; }

    public UsersZoneDb UsersZoneDb { get; set; }

    //public PermissionsZoneDb PermissionsZoneDb { get; set; }

    public ZoneDbManager(IServiceProvider provider)
    {
        UsedTokensZoneDb = provider.GetService<UsedTokensZoneDb>();

        UsersZoneDb = provider.GetService<UsersZoneDb>();

        //PermissionsZoneDb = provider.GetService<PermissionsZoneDb>();

        //LoadUserPermissions(provider);
    }

    //public void LoadUserPermissions(IServiceProvider provider)
    //{
    //    using var scope = provider.CreateScope();
    //    var context = scope.ServiceProvider.GetRequiredService<MagicTContext>();


    //    var roles = context.USER_ROLES
    //        .Include(x => x.AUTHORIZATIONS_BASE)
    //        .Select(x => new
    //        {
    //            userId = x.UR_USER_REFNO,
    //            PERMISSIONS = x.AUTHORIZATIONS_BASE is ROLES
    //                          ? ((ROLES)x.AUTHORIZATIONS_BASE).PERMISSIONS.Select(x=>x.PER_PERMISSION_NAME).ToList()
    //                          : new List<string> { ((PERMISSIONS)x.AUTHORIZATIONS_BASE).PER_PERMISSION_NAME }

    //        }).ToList();
               

    //    var grouped= roles.GroupBy(x => x.userId)
    //        .Select(g => new
    //        {
    //            UserId = g.Key,
    //            Permissions = g.SelectMany(x => x.PERMISSIONS).ToList()
    //        }).ToList();

    //    foreach (var usr in grouped)
    //    {
    //        PermissionsZoneDb.AddOrUpdate(usr.UserId, usr.Permissions);
    //    }

    //}
}

using AQueryMaker;
using MagicT.Server.Database;
using MagicT.Server.Reflection;
using MagicT.Server.ZoneTree;
using MagicT.Shared.Models;

namespace MagicT.Server.Initializers;

/// <summary>
/// Creates Roles and Permissions on database based on MagicServices.
/// </summary>
public class RolesAndPermissionsMigrator
{
    private MagicTContext Context { get; }

    public RolesAndPermissionsMigrator(MagicTContext context)
    {
        Context = context;
    }

    public void Migrate()
    {
        var localRoles = Context.ROLES.ToList();
        var localPermissions = Context.PERMISSIONS.ToList();

        var services = MagicServiceHelper.FindMagicServiceTypes();

        foreach (var service in services)
        {
            var existingRole = localRoles.Find(x => x.AB_NAME == service.Name);

            if (existingRole is not null)
            {
                foreach (var permissionName in MagicServiceHelper.FindMagicServiceMethods(service).Select(x=> x.Name))
                {
                    var existingPermission = localPermissions
                        .Find(x => x.AB_NAME ==permissionName);

                    if (existingPermission is not null) continue;

                    var newPermission = new PERMISSIONS
                    {
                        AB_NAME = permissionName,
                        PER_PERMISSION_NAME = existingRole.AB_NAME + "/" +permissionName.Replace("Async", ""),
                    };
                    
                    existingRole.PERMISSIONS.Add(newPermission);
                }
                continue;
            }

            var newRole = new ROLES
            {
                AB_NAME= service.Name, 
            };

            foreach (var permissionName in MagicServiceHelper.FindMagicServiceMethods(service).Select(x=> x.Name))
            {
                var newPermission = new PERMISSIONS
                {
                    PER_PERMISSION_NAME = newRole.AB_NAME + "/" + permissionName,
                    AB_NAME = permissionName
                };
                
                newRole.PERMISSIONS.Add(newPermission);
            }


            Context.ROLES.Add(newRole);
        }
        
        Context.SaveChanges();
    }
}
  

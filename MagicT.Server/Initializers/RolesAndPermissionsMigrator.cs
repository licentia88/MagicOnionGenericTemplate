using MagicT.Server.Database;
using MagicT.Server.Reflection;
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

        //Context.ROLES.RemoveRange(localRoles);

        //Context.SaveChanges();

        var services = MagicServiceHelper.FindMagicServiceTypes();

        foreach (var service in services)
        {
            var existingRole = localRoles.FirstOrDefault(x => x.AB_NAME == service.Name);

            if (existingRole is not null)
            {
                foreach (var permission in MagicServiceHelper.FindMagicServiceMethods(service))
                {
                    var existingPermission = localPermissions
                        .FirstOrDefault(x => x.AB_NAME == permission.Name);

                    if (existingPermission is not null) continue;

                    var newPermission = new PERMISSIONS
                    {
                        AB_NAME = permission.Name,
                        PER_PERMISSION_NAME = existingRole.AB_NAME + "/" +permission.Name.Replace("Async", ""),
                    };

                    //var validationContext = new ValidationContext(newPermission, null, null);
                    //Validator.TryValidateObject(newPermission, validationContext, default);

                    existingRole.PERMISSIONS.Add(newPermission);
                }
                continue;
            }

            var newRole = new ROLES
            {
                AB_NAME= service.Name, 
            };

            foreach (var permission in MagicServiceHelper.FindMagicServiceMethods(service))
            {
                var newPermission = new PERMISSIONS
                {
                    PER_PERMISSION_NAME = newRole.AB_NAME + "/" + permission.Name,
                    AB_NAME = permission.Name
                };

                //var validationContext = new ValidationContext(newPermission, null, null);
                //Validator.TryValidateObject(newPermission, validationContext,default);

                newRole.PERMISSIONS.Add(newPermission);
            }


            Context.ROLES.Add(newRole);
        }

        
        Context.SaveChanges();
    }
}
  

using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Humanizer;
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

        var localRoles = Context.ROLES_M.ToList();
        var localPermissions = Context.PERMISSIONS.ToList();

        //Context.ROLES_M.RemoveRange(localRoles);

        //Context.SaveChanges();

        var services = MagicServiceHelper.FindMagicServiceTypes();

        foreach (var service in services)
        {
            var exists = localRoles.FirstOrDefault(x => x.AB_NAME == service.Name);

            if (exists is not null)
            {
                foreach (var permission in MagicServiceHelper.FindMagicServiceMethods(service))
                {
                    var existingPermission = localPermissions
                        .FirstOrDefault(x => x.AB_NAME == permission.Name && x.PER_ROLE_NAME == exists.AB_NAME);

                    if (existingPermission is not null) continue;

                    var newPermission = new PERMISSIONS
                    {
                        AB_NAME = permission.Name,
                        PER_ROLE_NAME = exists.AB_NAME,
                        PER_PERMISSION_NAME = permission.Name.Replace("Async", ""),
                    };

                    var validationContext = new ValidationContext(newPermission, null, null);
                    Validator.TryValidateObject(newPermission, validationContext, default);

                    exists.ROLES_D.Add(new ROLES_D
                    {
                        PERMISSIONS = newPermission, RD_PERMISSION_REFNO = newPermission.AB_ROWID
                    });
                }
                continue;
            }

            var newRole = new ROLES_M
            {
                AB_NAME= service.Name, 
            };

            foreach (var permission in MagicServiceHelper.FindMagicServiceMethods(service))
            {
                var newPermission = new PERMISSIONS
                {
                    PER_PERMISSION_NAME = permission.Name,
                    PER_ROLE_NAME = newRole.AB_NAME,
                    AB_NAME = permission.Name};

                var validationContext = new ValidationContext(newPermission, null, null);
                Validator.TryValidateObject(newPermission, validationContext,default);


                newRole.ROLES_D.Add(new ROLES_D
                {
                     PERMISSIONS = newPermission,  RD_PERMISSION_REFNO = newPermission.AB_ROWID
                });
            }


            Context.ROLES_M.Add(newRole);
        }

        
        Context.SaveChanges();
    }
}
  

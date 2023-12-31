﻿using Benutomo;
using MagicT.Redis;
using MagicT.Server.Database;
using MagicT.Server.Reflection;
using MagicT.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Initializers;

/// <summary>
/// Creates Roles and Permissions on database based on MagicServices.
/// </summary>
[AutomaticDisposeImpl]
public partial class DataInitializer:IDisposable, IAsyncDisposable
{
    [EnableAutomaticDispose]
    private MagicTContext Context { get; }

    [EnableAutomaticDispose]
    public MagicTRedisDatabase MagicTRedisDatabase { get; set; }

    public DataInitializer(IServiceProvider provider)
    {
        Context = provider.GetService<MagicTContext>();
        MagicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();
    }

    public void Initialize()
    {
        AddOrUpdateRoles();

        AddRolesAndPermissionsToRedis();

        CreateAdmin();

        UpdateAdmin();
    }

    private void AddOrUpdateRoles()
    {
        List<ROLES> dbRoles = Context.ROLES.ToList();
        List<PERMISSIONS> dbPermissions = Context.PERMISSIONS.ToList();

        var currentServices = MagicServiceHelper.FindMagicServiceTypes();

        var enumerable = currentServices as Type[] ?? currentServices.ToArray();
        
        foreach (var service in enumerable)
        {
            var existingRole = dbRoles.Find(x => x.AB_NAME == service.Name);

            if (existingRole is not null)
            {
                foreach (var permissionName in MagicServiceHelper.FindMagicServiceMethods(service).Select(x=> x.Name))
                {
                    var existingPermission = dbPermissions
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

        RemoveStaleRolesAndPermissions(dbRoles, dbPermissions, enumerable);

        Context.SaveChanges();
    }

    private void RemoveStaleRolesAndPermissions(List<ROLES> roles, List<PERMISSIONS> permissions,IEnumerable<Type> services)
    {
        var enumerable = services as Type[] ?? services.ToArray();
        
        var currentMethods = enumerable.SelectMany(MagicServiceHelper.FindMagicServiceMethods);

        // Find roles in DB that don't have matching service
        var staleRoles = roles.Where(r => enumerable.All(s => s.Name != r.AB_NAME)).ToList();

        // Find permissions in DB with stale role or method
        var stalePermissions = permissions
          .Where(p => currentMethods.All(m => p.AB_NAME != m.Name))
          .ToList();

        // Remove stale roles and permissions
        Context.ROLES.RemoveRange(staleRoles);
        Context.PERMISSIONS.RemoveRange(stalePermissions);
    }

    private void AddRolesAndPermissionsToRedis()
    {
        var Roles = Context.PERMISSIONS.AsNoTracking().ToList();
 
        Roles.ForEach((PERMISSIONS per) => MagicTRedisDatabase.Create(Convert.ToString(per.PER_PERMISSION_NAME),per));
    }



    private void CreateAdmin()
    {
        if (Context.SUPER_USER.Any()) return;


        var admn = new SUPER_USER()
        {
            U_NAME = "admin",
            U_SURNAME = "admin",
            UB_PASSWORD = "admin",
            UB_FULLNAME= "admin",
            U_EMAIL = "admin@admin.com",
            U_PHONE_NUMBER = "xxxxx",
            UB_IS_ACTIVE = true
        };

       
          Context.SUPER_USER.Add(admn);

          Context.SaveChanges();
 
    }

    private void UpdateAdmin()
    {
        var admin = Context.SUPER_USER
            .Include(x => x.USER_ROLES)
            .FirstOrDefault(x => x.U_NAME == "admin");

        
        List<ROLES> dbRoles = Context.ROLES.ToList();

        // Create a HashSet of existing UR_ROLE_REFNO values
        if (admin != null)
        {
            var existingRoleRefNos = new HashSet<int>(admin.USER_ROLES.Select(ur => ur.UR_ROLE_REFNO));

            // Filter roles that are not already associated with the admin user
            var rolesToAdd = dbRoles.Where(role => !existingRoleRefNos.Contains(role.AB_ROWID)).ToList();

            // Add new roles to the user
            foreach (var role in rolesToAdd)
            {
                admin.USER_ROLES.Add(new USER_ROLES { UR_ROLE_REFNO = role.AB_ROWID });
            }
        }

        Context.SaveChanges(); // Save changes to the database
    }

 }


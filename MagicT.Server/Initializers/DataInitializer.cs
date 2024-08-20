using Benutomo;
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
public partial class DataInitializer : IDisposable, IAsyncDisposable
{
	private class Admin
	{
		public string Name { get; set; }
		
		public string Lastname { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }

		public string Email { get; set; }

		public string Phone { get; set; }
	}
	
	[EnableAutomaticDispose]
	private MagicTContext Context { get; }

	[EnableAutomaticDispose]
	public MagicTRedisDatabase MagicTRedisDatabase { get; set; }

	public IConfiguration Configuration { get; set; }

	public DataInitializer(IServiceProvider provider)
	{
		Context = provider.GetService<MagicTContext>();
		MagicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();
		Configuration = provider.GetService<IConfiguration>();
	}

	public void Initialize()
	{
		AddOrUpdateRoles();

		AddRolesAndPermissionsToRedis();

		CreateAdmins();

		UpdateAdmins();
	}

	private void AddOrUpdateRoles()
	{
		List<ROLES> dbRoles = Context.ROLES.ToList();
		List<PERMISSIONS> dbPermissions = Context.PERMISSIONS.ToList();

		var currentServices = MagicServiceHelper.FindMagicServiceTypes();

		//var enumerable = currentServices as Type[] ?? currentServices.ToArray();

		foreach (var service in currentServices)
		{
			var existingRole = dbRoles.Find(x => x.AB_NAME == service.Name);

			var services = MagicServiceHelper.FindMagicServiceMethods(service);

            if (existingRole is not null)
			{
				foreach (var permissionName in services.Select(x => x.Name))
				{
					var existingPermission = dbPermissions.FirstOrDefault(x => x.AB_NAME == permissionName && x.PER_PERMISSION_NAME == $"{service.Name}/{permissionName}");

					if (existingPermission is not null) continue;

					var newPermission = new PERMISSIONS
					{
						AB_NAME = permissionName,
						PER_PERMISSION_NAME = existingRole.AB_NAME + "/" + permissionName,
					};

					existingRole.PERMISSIONS.Add(newPermission);
				}
				continue;
			}

			var newRole = new ROLES
			{
				AB_NAME = service.Name,
			};

			foreach (var permissionName in services.Select(x => x.Name))
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

		RemoveStaleRolesAndPermissions(dbRoles, dbPermissions, currentServices);

		Context.SaveChanges();
	}

	private void RemoveStaleRolesAndPermissions(List<ROLES> roles, List<PERMISSIONS> permissions, IEnumerable<Type> services)
	{
		//var enumerable = services as Type[] ?? services.ToArray();

		var currentMethods = services.SelectMany(MagicServiceHelper.FindMagicServiceMethods);

		// Find roles in DB that don't have matching service
		var staleRoles = roles.Where(r => services.All(s => s.Name != r.AB_NAME)).ToList();

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
		var roles = Context.PERMISSIONS.AsNoTracking().ToList();

		roles.ForEach((PERMISSIONS per) => MagicTRedisDatabase.Create(Convert.ToString(per.PER_PERMISSION_NAME), per));
	}

	private void CreateAdmins()
	{
		List<Admin> admins = Configuration.GetSection("Admins").Get<List<Admin>>();

		var existingAdmins = Context.USERS.ToList();

		foreach (var admin in admins)
		{
			var existingAdmin = existingAdmins.FirstOrDefault(x => x.U_USERNAME == admin.Username);
			if (existingAdmin is not null) continue;
 
			var newAdmin = new USERS
			{
				U_USERNAME = admin.Username,
				U_NAME = admin.Name,
				U_LASTNAME = admin.Lastname,
				U_PASSWORD = admin.Password,
				U_EMAIL =  admin.Email,
				U_FULLNAME = $"{admin.Name} {admin.Lastname}",
                U_PHONE_NUMBER = admin.Phone,
				U_IS_ACTIVE = true,
				U_IS_ADMIN = true
			};

			Context.USERS.Add(newAdmin);
		}

		Context.SaveChanges();
	}

	private void UpdateAdmins()
	{
		List<Admin> admins = Configuration.GetSection("Admins").Get<List<Admin>>();

		var superUsers = Context.USERS
			.Include(x => x.USER_ROLES).Where(x => admins.Select(y => y.Username).Contains(x.U_USERNAME)).ToList();

		List<ROLES> dbRoles = Context.ROLES.ToList();

		foreach (var superUser in superUsers)
		{
			var existingRoleRefNos = new HashSet<int>(superUser.USER_ROLES.Select(ur => ur.UR_ROLE_REFNO));

			// Filter roles that are not already associated with the admin user
			var rolesToAdd = dbRoles.Where(role => !existingRoleRefNos.Contains(role.AB_ROWID)).ToList();

			// Add new roles to the user
			foreach (var role in rolesToAdd)
			{
				superUser.USER_ROLES.Add(new USER_ROLES { UR_ROLE_REFNO = role.AB_ROWID });
			}

			Context.SaveChanges();
		}
	}
}


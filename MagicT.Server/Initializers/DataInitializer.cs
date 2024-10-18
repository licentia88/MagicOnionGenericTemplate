using System.Reflection;
using Benutomo;
using MagicT.Redis;
using MagicT.Server.Reflection;
using MagicT.Shared.Models;

namespace MagicT.Server.Initializers;

/// <summary>
/// Creates Roles and Permissions on database based on MagicServices.
/// </summary>
[AutomaticDisposeImpl]
public partial class DataInitializer : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Represents an admin user.
    /// </summary>
    private class Admin
    {
        public Admin(string name, string lastname, string username, string password, string email, string phone)
        {
            Name = name;
            Lastname = lastname;
            Username = username;
            Password = password;
            Email = email;
            Phone = phone;
        }

        /// <summary>
        /// Gets or sets the admin's first name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the admin's last name.
        /// </summary>
        public string Lastname { get; }

        /// <summary>
        /// Gets or sets the admin's username.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Gets or sets the admin's password.
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// Gets or sets the admin's email address.
        /// </summary>
        public string Email { get; }

        /// <summary>
        /// Gets or sets the admin's phone number.
        /// </summary>
        public string Phone { get; }
    }

    /// <summary>
    /// Gets the database context.
    /// </summary>
    [EnableAutomaticDispose]
    private MagicTContext Context { get; }

    /// <summary>
    /// Gets or sets the Redis database.
    /// </summary>
    [EnableAutomaticDispose]
    public MagicTRedisDatabase MagicTRedisDatabase { get; set; }

    /// <summary>
    /// Gets or sets the configuration.
    /// </summary>
    public IConfiguration Configuration { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataInitializer"/> class.
    /// </summary>
    /// <param name="provider">The service provider for dependency resolution.</param>
    public DataInitializer(IServiceProvider provider)
    {
        Context = provider.GetService<MagicTContext>();
        MagicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();
        Configuration = provider.GetService<IConfiguration>();
    }

    /// <summary>
    /// Initializes the data by adding or updating roles, adding roles and permissions to Redis, and creating or updating admins.
    /// </summary>
    public void Initialize()
    {
        AddOrUpdateRoles();
        AddRolesAndPermissionsToRedis();
        CreateOrUpdateAdmins();
    }

    /// <summary>
    /// Adds or updates roles in the database.
    /// </summary>
    private void AddOrUpdateRoles()
    {
        var dbRoles = Context.ROLES.ToList();
        var dbPermissions = Context.PERMISSIONS.ToList();
        var currentServices = MagicServiceHelper.FindMagicServiceTypes();

        var enumerable = currentServices as Type[] ?? currentServices.ToArray();
        foreach (var service in enumerable)
        {
            var existingRole = dbRoles.Find(x => x.AB_NAME == service.Name);
            var serviceMethods = MagicServiceHelper.FindMagicServiceMethods(service);

            if (existingRole != null)
            {
                AddPermissionsToRole(existingRole, serviceMethods, dbPermissions);
            }
            else
            {
                var newRole = new ROLES { AB_NAME = service.Name };
                AddPermissionsToRole(newRole, serviceMethods);
                Context.ROLES.Add(newRole);
            }
        }

        RemoveStaleRolesAndPermissions(dbRoles, dbPermissions, enumerable);
        Context.SaveChanges();
    }

    /// <summary>
    /// Adds permissions to a role.
    /// </summary>
    /// <param name="role">The role to add permissions to.</param>
    /// <param name="methods">The methods representing the permissions.</param>
    /// <param name="dbPermissions">The existing permissions in the database.</param>
    private void AddPermissionsToRole(ROLES role, IEnumerable<MethodInfo> methods, List<PERMISSIONS> dbPermissions = null)
    {
        foreach (var method in methods)
        {
            if (dbPermissions != null)
            {
                var existingPermission = dbPermissions.FirstOrDefault(x => x.AB_NAME == method.Name && x.PER_PERMISSION_NAME == $"{role.AB_NAME}/{method.Name}");
                if (existingPermission != null) continue;
            }

            var newPermission = new PERMISSIONS
            {
                AB_NAME = method.Name,
                PER_PERMISSION_NAME = $"{role.AB_NAME}/{method.Name}"
            };
            role.PERMISSIONS.Add(newPermission);
        }
    }

    /// <summary>
    /// Removes stale roles and permissions from the database.
    /// </summary>
    /// <param name="roles">The list of roles in the database.</param>
    /// <param name="permissions">The list of permissions in the database.</param>
    /// <param name="services">The current services.</param>
    private void RemoveStaleRolesAndPermissions(List<ROLES> roles, List<PERMISSIONS> permissions, IEnumerable<Type> services)
    {
        var enumerable = services as Type[] ?? services.ToArray();
        var currentMethods = enumerable.SelectMany(MagicServiceHelper.FindMagicServiceMethods);
        var staleRoles = roles.Where(r => enumerable.All(s => s.Name != r.AB_NAME)).ToList();
        var stalePermissions = permissions.Where(p => currentMethods.All(m => p.AB_NAME != m.Name)).ToList();

        Context.ROLES.RemoveRange(staleRoles);
        Context.PERMISSIONS.RemoveRange(stalePermissions);
    }

    /// <summary>
    /// Adds roles and permissions to Redis.
    /// </summary>
    private void AddRolesAndPermissionsToRedis()
    {
        var roles = Context.PERMISSIONS.AsNoTracking().ToList();
        roles.ForEach(per => MagicTRedisDatabase.Create(per.PER_PERMISSION_NAME, per));
    }

    /// <summary>
    /// Creates or updates admin users.
    /// </summary>
    private void CreateOrUpdateAdmins()
    {
        var admins = Configuration.GetSection("Admins").Get<List<Admin>>();
        var existingAdmins = Context.USERS.Include(x=>x.USER_ROLES).ToList();

        foreach (var admin in admins)
        {
            var existingAdmin = existingAdmins.FirstOrDefault(x => x.U_USERNAME == admin.Username);
            if (existingAdmin == null)
            {
                CreateAdmin(admin);
            }
            else
            {
                UpdateAdminRoles(existingAdmin);
            }
        }

        Context.SaveChanges();
    }

    /// <summary>
    /// Creates a new admin user.
    /// </summary>
    /// <param name="admin">The admin details.</param>
    private void CreateAdmin(Admin admin)
    {
        var newAdmin = new USERS
        {
            U_USERNAME = admin.Username,
            U_NAME = admin.Name,
            U_LASTNAME = admin.Lastname,
            U_PASSWORD = admin.Password,
            U_EMAIL = admin.Email,
            U_FULLNAME = $"{admin.Name} {admin.Lastname}",
            U_PHONE_NUMBER = admin.Phone,
            U_IS_ACTIVE = true,
            U_IS_ADMIN = true
        };
        AssignRolesToAdmin(newAdmin);
        Context.USERS.Add(newAdmin);
    }

    /// <summary>
    /// Updates the roles of an existing admin user.
    /// </summary>
    /// <param name="admin">The existing admin user.</param>
    private void UpdateAdminRoles(USERS admin)
    {
        AssignRolesToAdmin(admin);
    }

    /// <summary>
    /// Assigns roles to an admin user.
    /// </summary>
    /// <param name="admin">The admin user.</param>
    private void AssignRolesToAdmin(USERS admin)
    {
        var dbRoles = Context.ROLES.ToList();
        var existingRoleRefNos = new HashSet<int>(admin.USER_ROLES.Select(ur => ur.UR_ROLE_REFNO));
        var rolesToAdd = dbRoles.Where(role => !existingRoleRefNos.Contains(role.AB_ROWID)).ToList();

        foreach (var role in rolesToAdd)
        {
            admin.USER_ROLES.Add(new USER_ROLES { UR_ROLE_REFNO = role.AB_ROWID });
        }
        
        
    }
}
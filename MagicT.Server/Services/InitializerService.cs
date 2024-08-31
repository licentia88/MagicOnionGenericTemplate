using Benutomo;
using MagicOnion;
using MagicT.Server.Enums;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

/// <summary>
/// Service for initializing and retrieving various entities such as permissions, roles, users, and operations.
/// </summary>
[AutomaticDisposeImpl]
// ReSharper disable once UnusedType.Global
public partial class InitializerService : MagicServerBase<IInitializerService>, IInitializerService
{
    /// <summary>
    /// Gets or sets the database context.
    /// </summary>
    [EnableAutomaticDispose]
    private MagicTContext Db { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializerService"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public InitializerService(IServiceProvider provider) : base(provider)
    {
        Db = provider.GetService<MagicTContext>();
    }

    /// <summary>
    /// Retrieves a list of permissions.
    /// </summary>
    /// <returns>A <see cref="UnaryResult{T}"/> containing a list of <see cref="PERMISSIONS"/>.</returns>
    public async UnaryResult<List<PERMISSIONS>> GetPermissions()
    {
        return await ExecuteAsync(async () => await Db.PERMISSIONS.ToListAsync());
    }

    /// <summary>
    /// Retrieves a list of roles.
    /// </summary>
    /// <returns>A <see cref="UnaryResult{T}"/> containing a list of <see cref="ROLES"/>.</returns>
    public async UnaryResult<List<ROLES>> GetRoles()
    {
        return await ExecuteAsync(async () => await Db.ROLES.ToListAsync());
    }

    /// <summary>
    /// Retrieves a list of users.
    /// </summary>
    /// <returns>A <see cref="UnaryResult{T}"/> containing a list of <see cref="USERS"/>.</returns>
    public async UnaryResult<List<USERS>> GetUsers()
    {
        return await ExecuteAsync(async () => await Db.USERS.ToListAsync());
    }

    /// <summary>
    /// Retrieves a list of operations.
    /// </summary>
    /// <returns>A <see cref="UnaryResult{T}"/> containing a list of <see cref="Operations"/>.</returns>
    public async UnaryResult<List<Operations>> GetOperations()
    {
        return await ExecuteAsync(() =>
        {
            var values = Enum.GetValues<AuditType>();

            return values.Select((op, val) => new Operations { Id = val, Description = op.ToString() }).ToList();
        });
    }
}
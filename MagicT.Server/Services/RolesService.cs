using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

/// <summary>
/// Service for handling role-related operations.
/// </summary>
// ReSharper disable once UnusedType.Global
public class RolesService : MagicServerSecureService<IRolesService, ROLES, MagicTContext>, IRolesService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RolesService"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public RolesService(IServiceProvider provider) : base(provider)
    {
    }
}
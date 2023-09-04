using MagicOnion;
using MagicOnion.Client;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Models.Base;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

/// <summary>
/// This is the client-side implementation of the <see cref="IRolesService"/> interface.
/// </summary>
public sealed class RolesService : MagicClientServiceBase<IRolesService, ROLES>, IRolesService
{
    /// <inheritdoc />
    public RolesService(IServiceProvider provider) : base(provider)
    {
    }
}
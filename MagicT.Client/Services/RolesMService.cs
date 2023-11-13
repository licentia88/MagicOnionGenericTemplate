using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

/// <summary>
/// This is the client-side implementation of the <see cref="IRolesService"/> interface.
/// </summary>
public sealed class RolesService : MagicClientService<IRolesService, ROLES>, IRolesService
{
    /// <inheritdoc />
    public RolesService(IServiceProvider provider) : base(provider)
    {
    }
}
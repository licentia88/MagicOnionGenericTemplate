using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

/// <summary>
/// This is the client-side implementation of the <see cref="IRolesService"/> interface.
/// </summary>
[RegisterScoped]
public sealed class RolesService : MagicClientSecureService<IRolesService, ROLES>, IRolesService
{
    /// <inheritdoc />
    public RolesService(IServiceProvider provider) : base(provider)
    {
    }
}
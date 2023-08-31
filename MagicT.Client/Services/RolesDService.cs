using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

/// <summary>
/// This is the client-side implementation of the <see cref="IRolesDService"/> interface.
/// </summary>
public sealed class RolesDService : MagicClientServiceBase<IRolesDService, ROLES_D>, IRolesDService
{
    /// <inheritdoc />
    public RolesDService(IServiceProvider provider) : base(provider)
    {
    }
}
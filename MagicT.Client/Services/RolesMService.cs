using MagicOnion.Client;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

/// <summary>
/// This is the client-side implementation of the <see cref="IRolesMService"/> interface.
/// </summary>
public sealed class RolesMService : MagicClientServiceBase<IRolesMService, ROLES_M>, IRolesMService
{
    /// <inheritdoc />
    public RolesMService(IServiceProvider provider) : base(provider)
    {
    }
}
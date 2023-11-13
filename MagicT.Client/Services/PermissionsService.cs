using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

public sealed class PermissionsService : MagicClientService<IPermissionsService, PERMISSIONS>, IPermissionsService
{
    public PermissionsService(IServiceProvider provider) : base(provider)
    {
    }
}


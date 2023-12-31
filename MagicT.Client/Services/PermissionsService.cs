using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

[RegisterScoped]
public sealed class PermissionsService : MagicClientSecureService<IPermissionsService, PERMISSIONS>, IPermissionsService
{
    public PermissionsService(IServiceProvider provider) : base(provider)
    {
    }
}


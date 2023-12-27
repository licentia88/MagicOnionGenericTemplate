using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

/// <summary>
/// User service
/// </summary>
[RegisterScoped]
public sealed class UserService : MagicClientSecureService<IUserService, USERS>, IUserService
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="filters"></param>
    public UserService(IServiceProvider provider) : base(provider)
    {
    }
}

using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public sealed partial class UserService : MagicServerAuthService<IUserService, USERS, MagicTContext>, IUserService
{
    public UserService(IServiceProvider provider) : base(provider)
    {
    }
}
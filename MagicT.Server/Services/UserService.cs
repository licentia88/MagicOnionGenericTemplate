using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public partial class UserService : MagicServerSecureService<IUserService, USERS, MagicTContext>, IUserService
{
    public UserService(IServiceProvider provider) : base(provider)
    {
    }

}
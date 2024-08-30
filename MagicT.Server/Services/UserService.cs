using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public partial class UserService : MagicServerSecureService<IUserService, USERS, MagicT.Server.Database.MagicTContext>, IUserService
{
    public UserService(IServiceProvider provider) : base(provider)
    {
        //var person = A.New<USERS>();

    }


    public override async UnaryResult<List<USERS>> FindByParametersAsync(byte[] parameters)
    {
        return  await base.FindByParametersAsync(parameters);
    }
}
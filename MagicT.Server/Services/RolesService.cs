using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Enums;
using MagicT.Server.Extensions;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;
using Mapster;

namespace MagicT.Server.Services;

public class RolesService : MagicServerService<IRolesService, ROLES>, IRolesService
{
    public RolesService(IServiceProvider provider) : base(provider)
    {

        Dictionary<string, object> myDIction = new Dictionary<string, object>();

      

    }

}

 
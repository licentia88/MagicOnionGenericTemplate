using MagicOnion;
using MagicT.Server.Filters;
using MagicT.Shared.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services.Base;

[MagicTAuthorize]
public class AuthorizationSeviceBase<TService, TModel, TContext> : MagicTServerServiceBase<TService, TModel, TContext>, IMagicTService<TService, TModel>
    where TService : IMagicTService<TService, TModel>, IService<TService>
    where TModel : class
    where TContext : DbContext
{

    public AuthorizationSeviceBase(IServiceProvider provider) : base(provider)
    {
    }
    
}

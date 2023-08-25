using MagicOnion;
using MagicT.Server.Filters;
using MagicT.Shared.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services.Base;

[MagicTAuthorize]
public class AuthorizationSeviceBase<TService, TModel, TContext> : MagicServerServiceBase<TService, TModel, TContext>, IMagicService<TService, TModel>
    where TService : IMagicService<TService, TModel>, IService<TService>
    where TModel : class
    where TContext : DbContext
{

    public AuthorizationSeviceBase(IServiceProvider provider) : base(provider)
    {
    }
    
}

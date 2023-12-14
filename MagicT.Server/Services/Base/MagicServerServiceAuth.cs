using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Filters;
using MagicT.Shared.Services.Base;

namespace MagicT.Server.Services.Base;

[MagicTAuthorize]

public abstract class MagicServerServiceAuth<TService, TModel, TContext> : DatabaseService<TService, TModel, MagicTContext>
    where TService : IMagicService<TService, TModel>, IService<TService>
    where TModel : class
    where TContext : MagicTContext
{


    public MagicServerServiceAuth(IServiceProvider provider) : base(provider)
    {
    }

 
 
 
  
    
}

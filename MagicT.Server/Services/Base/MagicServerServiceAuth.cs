using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Filters;
using MagicT.Shared.Services.Base;

namespace MagicT.Server.Services.Base;

[MagicTAuthorize]

public abstract class MagicServerServiceAuth<TService, TModel, TContext> : AuditDatabaseService<TService, TModel, MagicTContext>
    where TService : IMagicService<TService, TModel>, IService<TService>
    where TModel : class
    where TContext : MagicTContext
{


    // ReSharper disable once PublicConstructorInAbstractClass
    public MagicServerServiceAuth(IServiceProvider provider) : base(provider)
    {
    }

 
 
 
  
    
}

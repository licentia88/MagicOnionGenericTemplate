using MagicOnion;
using MagicT.Server.Filters;
using MagicT.Shared.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services.Base;

[MagicTAuthorize]
public abstract class MagicServerAuthService<TService, TModel, TContext> : AuditDatabaseService<TService, TModel, TContext>
    where TService : IMagicService<TService, TModel>, IService<TService>
    where TModel : class
    where TContext : DbContext
{


    // ReSharper disable once PublicConstructorInAbstractClass
    public MagicServerAuthService(IServiceProvider provider) : base(provider)
    {
    }

 
 
 
  
    
}

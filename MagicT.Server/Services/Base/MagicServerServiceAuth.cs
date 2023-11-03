using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Filters;
using MagicT.Shared.Services.Base;
using Microsoft.EntityFrameworkCore.Storage;

namespace MagicT.Server.Services.Base;

[MagicTAuthorize]
public abstract class MagicServerServiceAuth<TService, TModel, TContext> : DatabaseService<TService, TModel, MagicTContextAudit>
    where TService : IMagicService<TService, TModel>, IService<TService>
    where TModel : class
    where TContext : MagicTContextAudit
{

    public MagicServerServiceAuth(IServiceProvider provider) : base(provider)
    {
    }

 
    public override void Execute(Action task)
    {
        Database.UserId = CurrentUser(Context);
        base.Execute(task);
    }

    public override Task ExecuteAsync(Func<Task> task)
    {
        Database.UserId = CurrentUser(Context);
        return base.ExecuteAsync(task);
    }

    public override UnaryResult<T> ExecuteWithoutResponse<T>(Func<T> task, IDbContextTransaction transaction)
    {
        Database.UserId = CurrentUser(Context);
        return base.ExecuteWithoutResponse(task, transaction);
    }

    public override UnaryResult<T> ExecuteWithoutResponseAsync<T>(Func<Task<T>> task, IDbContextTransaction transaction)
    {
        Database.UserId = CurrentUser(Context);
        return base.ExecuteWithoutResponseAsync(task, transaction);
    }
}

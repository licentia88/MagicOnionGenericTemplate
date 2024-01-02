using Benutomo;
using Coravel.Queuing.Interfaces;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using MagicT.Redis;
using MagicT.Server.Exceptions;
using MagicT.Server.Extensions;
using MagicT.Server.Jwt;
using MagicT.Server.Managers;
using MagicT.Server.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace MagicT.Server.Services.Base;

[AutomaticDisposeImpl]
public abstract partial class MagicServerBase<TService> : ServiceBase<TService> ,IDisposable,IAsyncDisposable where TService : IService<TService>
{
    protected readonly IQueue Queue;

    protected DbExceptionHandler DbExceptionHandler { get; set; }
    
    [EnableAutomaticDispose]
    protected MagicTRedisDatabase MagicTRedisDatabase { get; set; }

    [EnableAutomaticDispose]
    protected CancellationTokenManager CancellationTokenManager { get; set; }

    protected MagicTToken Token => Context.GetItemAs<MagicTToken>(nameof(MagicTToken));

    protected int CurrentUserId => Token?.Id ?? 0;
    
    protected byte[] SharedKey => MagicTRedisDatabase.ReadAs<UsersCredentials>(CurrentUserId.ToString()).SharedKey;

    [EnableAutomaticDispose]
    protected IDbContextTransaction Transaction;

    public MagicServerBase(IServiceProvider provider)
    {
        Queue = provider.GetService<IQueue>();

        MagicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();

        CancellationTokenManager = provider.GetService<CancellationTokenManager>();

        DbExceptionHandler = provider.GetService<DbExceptionHandler>();
    }

    protected virtual async UnaryResult<T> ExecuteAsync<T>(Func<Task<T>> task) 
    {
        try
        {          
            var result = await task().ConfigureAwait(false);

            if (Transaction is not null)
                await Transaction.CommitAsync();

            return result;
        }
        catch (Exception ex)
        {
            if (Transaction is not null)
                await Transaction.RollbackAsync();

            throw new ReturnStatusException(StatusCode.Cancelled, HandleException(ex));
        }

    }

    public virtual UnaryResult<T> Execute<T>(Func<T> task)
    {
        try
        {
            var result = task();

            if (Transaction is not null)
                Transaction.Commit();

            return UnaryResult.FromResult(result);
        }
        catch (Exception ex)
        {
            if (Transaction is not null)
                 Transaction.Rollback();

            throw new ReturnStatusException(StatusCode.Cancelled, HandleException(ex));
        }
    }

    /// <summary>
    ///     Executes an action.
    /// </summary>
    /// <param name="task">The action to execute.</param>
    public virtual void Execute(Action task)
    {
        try
        {
            task();

            if (Transaction is not null)
                 Transaction.Commit();
        }
        catch (Exception ex)
        {
            if (Transaction is not null)
                 Transaction.Rollback();

            throw new ReturnStatusException(StatusCode.Cancelled, HandleException(ex));
        }
    }

    /// <summary>
    /// Executes an asynchronous task without returning a response.
    /// </summary>
    /// <param name="task">The asynchronous task to execute.</param>
    public virtual async Task ExecuteAsync(Func<Task> task)
    {

        try
        {
            await task().ConfigureAwait(false);

            if (Transaction is not null)
                await Transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            if (Transaction is not null)
                await Transaction.RollbackAsync();

            throw new ReturnStatusException(StatusCode.Cancelled, HandleException(ex));
        }
         
    }

    private string HandleException(Exception ex)
    {
        //Logger.Log(LogLevel.Error, ex.Message);
        return DbExceptionHandler.HandleException(ex);
    }
 
}

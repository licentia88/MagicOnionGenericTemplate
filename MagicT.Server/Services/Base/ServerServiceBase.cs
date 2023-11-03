using Coravel.Queuing.Interfaces;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using MagicT.Server.Exceptions;
using MagicT.Server.Extensions;
using MagicT.Server.Jwt;
using MagicT.Shared.Services.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MagicT.Server.Services.Base;



public abstract class ServerServiceBase<TService, TModel, TContext> : ServiceBase<TService>
    where TService : IMagicService<TService, TModel>, IService<TService>
    where TModel : class
    where TContext : DbContext
{
    protected readonly IQueue _queue;

    private ILogger<TModel> Logger { get; set; }

    protected DbExceptionHandler DbExceptionHandler { get; set; }

    protected MagicTToken Token(ServiceContext context) => Context.GetItemAs<MagicTToken>(nameof(MagicTToken));

    public ServerServiceBase(IServiceProvider provider)
    {
        _queue = provider.GetService<IQueue>();

        Logger = provider.GetService<ILogger<TModel>>();

        DbExceptionHandler = provider.GetService<DbExceptionHandler>();
    }

    /// <summary>
    ///     Executes an asynchronous task without returning a response.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The asynchronous task to execute.</param>
    /// <returns>A <see cref="UnaryResult{T}" /> containing the result of the task.</returns>
    public virtual UnaryResult<T> ExecuteWithoutResponseAsync<T>(Func<Task<T>> task)
    {
        return ExecuteWithoutResponseAsync(task, null);
    }

    public virtual async UnaryResult<T> ExecuteWithoutResponseAsync<T>(Func<Task<T>> task, IDbContextTransaction transaction)
    {
        try
        {
            var result = await task().ConfigureAwait(false);

            if (transaction is not null)
                await transaction?.CommitAsync();

            return result;
        }
        catch (Exception ex)
        {
            if (transaction is not null)
                await transaction?.RollbackAsync();

            throw new ReturnStatusException(StatusCode.Cancelled, HandleException(ex));
        }

    }

    /// <summary>
    ///     Executes a synchronous task without returning a response.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The synchronous task to execute.</param>
    /// <returns>A <see cref="UnaryResult{T}" /> containing the result of the task.</returns>
    public virtual UnaryResult<T> ExecuteWithoutResponse<T>(Func<T> task)
    {
        return ExecuteWithoutResponse(task, null);
    }

    public virtual UnaryResult<T> ExecuteWithoutResponse<T>(Func<T> task, IDbContextTransaction transaction)
    {
        try
        {
            var result = task();

            if (transaction is not null)
                transaction?.Commit();

            return UnaryResult.FromResult(result);
        }
        catch (Exception ex)
        {
            if (transaction is not null)
                transaction?.Rollback();

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
        }
        catch (Exception ex)
        {
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
        }
        catch (Exception ex)
        {
            throw new ReturnStatusException(StatusCode.Cancelled, HandleException(ex));
        }
         
    }


    protected int CurrentUser(ServiceContext context) => Token(context).Id;


    private string HandleException(Exception ex)
    {
        Logger.Log(LogLevel.Error, ex.Message);
        return DbExceptionHandler.HandleException(ex);
    }
}

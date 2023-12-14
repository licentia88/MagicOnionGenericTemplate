using Coravel.Queuing.Interfaces;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using MagicT.Redis;
using MagicT.Server.Exceptions;
using MagicT.Server.Extensions;
using MagicT.Server.Jwt;
using MagicT.Server.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace MagicT.Server.Services.Base;


public abstract class MagicServerBase<TService> : ServiceBase<TService> where TService : IService<TService>
{
    protected readonly IQueue Queue;

    protected DbExceptionHandler DbExceptionHandler { get; set; }


    /// <summary>
    /// ZoneTree Database Manager
    /// </summary>
    public MagicTRedisDatabase MagicTRedisDatabase { get; set; }

    public MagicTToken Token => Context.GetItemAs<MagicTToken>(nameof(MagicTToken));

    public int CurrentUserId => Token == null ? 0 : Token.Id;

    public byte[] SharedKey => GetSharedKey();

    private byte[] GetSharedKey()
    {
        var key = Convert.ToString(CurrentUserId);

        var currentUser =  MagicTRedisDatabase.ReadAs<UsersCredentials>(key);
        //var currentUser = CredentialsContext.UsersCredentials.Find(CurrentUserId);

        return currentUser.SharedKey;
        //var result = ZoneDbManager.UsersZoneDb.Database.TryGet(CurrentUserId, out UsersZone user);

        //if (result)
        //    return user.SharedKey;

        //throw new ReturnStatusException(StatusCode.NotFound, "Shared Key not found");
    }
       
 


    public MagicServerBase(IServiceProvider provider)
    {
        Queue = provider.GetService<IQueue>();

        MagicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();

        DbExceptionHandler = provider.GetService<DbExceptionHandler>();
    }

    /// <summary>
    ///     Executes an asynchronous task without returning a response.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The asynchronous task to execute.</param>
    /// <returns>A <see cref="UnaryResult{T}" /> containing the result of the task.</returns>
    public UnaryResult<T> ExecuteWithoutResponseAsync<T>(Func<Task<T>> task)
    {
        return ExecuteWithoutResponseAsync(task, null);
    }

    public async UnaryResult<T> ExecuteWithoutResponseAsync<T>(Func<Task<T>> task, IDbContextTransaction transaction)
    {
        try
        {
            var result = await task().ConfigureAwait(false);

            if (transaction is not null)
            {
                await transaction?.CommitAsync();
                
            }

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
    public UnaryResult<T> ExecuteWithoutResponse<T>(Func<T> task)
    {
        return ExecuteWithoutResponse(task, null);
    }

    public UnaryResult<T> ExecuteWithoutResponse<T>(Func<T> task, IDbContextTransaction transaction)
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
    public void Execute(Action task)
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
    public async Task ExecuteAsync(Func<Task> task)
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




    private string HandleException(Exception ex)
    {
        //Logger.Log(LogLevel.Error, ex.Message);
        return DbExceptionHandler.HandleException(ex);
    }


  

}

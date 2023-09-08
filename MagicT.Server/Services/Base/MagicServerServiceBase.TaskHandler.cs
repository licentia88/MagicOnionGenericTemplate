using System.Data;
using System.Runtime.CompilerServices;
using Grpc.Core;
using MagicOnion;
using MagicT.Server.Exceptions;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ServiceModels;
using Microsoft.EntityFrameworkCore.Storage;

namespace MagicT.Server.Services.Base;

public abstract partial class MagicServerServiceBase<TService, TModel, TContext>  
{
    public ILogger<TModel> Logger { get; set; }

    public DbExceptionHandler DbExceptionHandler { get; set; }

    public bool IgnoreTransaction { get; set; }

    /// <summary>
    ///     Executes an asynchronous task and wraps the result in a <see cref="RESPONSE_RESULT{TModel}" /> object.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The asynchronous task to execute.</param>
    /// <returns>A <see cref="UnaryResult{T}" /> containing the result of the task.</returns>
    public  async UnaryResult<RESPONSE_RESULT<T>> ExecuteAsync<T>(Func<Task<T>> task, [CallerMemberName] string methodName = default) where T : new()
    {
        IDbContextTransaction transaction = default;

        if (!IgnoreTransaction)
            transaction = Db.Database.CurrentTransaction ?? await Db.Database.BeginTransactionAsync();

        using (transaction)
        {
            try
            {
                var result = await task().ConfigureAwait(false);

                var responseData = new RESPONSE_RESULT<T>(result);

                if (!IgnoreTransaction)
                    await transaction.CommitAsync();

                return responseData;
            }
            catch (Exception ex)
            {
                if (!IgnoreTransaction)
                    await transaction.RollbackAsync();

                throw new ReturnStatusException(StatusCode.Cancelled, HandleException(ex));
            }
        }
      
         
    }

    /// <summary>
    ///     Executes a synchronous task and wraps the result in a <see cref="RESPONSE_RESULT{T}" /> object.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The synchronous task to execute.</param>
    /// <returns>A <see cref="UnaryResult{T}" /> containing the result of the task.</returns>
    public UnaryResult<RESPONSE_RESULT<T>> Execute<T>(Func<T> task, [CallerMemberName] string methodName = default) where T : new()
    {
        try
        {
            var result = task();
            return new UnaryResult<RESPONSE_RESULT<T>>(new RESPONSE_RESULT<T>(result));
        }
        catch (Exception ex)
        {
            throw new ReturnStatusException(StatusCode.Cancelled, HandleException(ex));
        }
    }

    /// <summary>
    ///     Executes an asynchronous task without returning a response.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The asynchronous task to execute.</param>
    /// <returns>A <see cref="UnaryResult{T}" /> containing the result of the task.</returns>
    public async UnaryResult<T> ExecuteAsyncWithoutResponse<T>(Func<Task<T>> task, [CallerMemberName] string methodName = default)
    {

        try
        {
            var result = await task().ConfigureAwait(false);

            //if (!IgnoreTransaction)
            //    await transaction.CommitAsync();

            return result;
        }
        catch (Exception ex)
        {
            //if (!IgnoreTransaction)
                //await transaction.RollbackAsync();

            throw new ReturnStatusException(StatusCode.Cancelled, HandleException(ex));
        }

        //IDbContextTransaction transaction = default;

        //if (!IgnoreTransaction)
        //    transaction = Db.Database.CurrentTransaction ?? await Db.Database.BeginTransactionAsync();


        //using (transaction)
        //{
            
        //}

     
    }

    /// <summary>
    ///     Executes a synchronous task without returning a response.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The synchronous task to execute.</param>
    /// <returns>A <see cref="UnaryResult{T}" /> containing the result of the task.</returns>
    public UnaryResult<T> ExecuteWithoutResponse<T>(Func<T> task, [CallerMemberName] string methodName = default)
    {
        try
        {
            var result = task();
            return new UnaryResult<T>(result);
        }
        catch (Exception ex)
        {
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
    ///     Executes an asynchronous task without returning a response.
    /// </summary>
    /// <param name="task">The asynchronous task to execute.</param>
    public async Task ExecuteAsync(Func<Task> task, [CallerMemberName] string methodName = default)
    {

        IDbContextTransaction transaction = default;

        if (!IgnoreTransaction)
            transaction = Db.Database.CurrentTransaction ?? await Db.Database.BeginTransactionAsync();

        using (transaction)
        {
            try
            {
                await task().ConfigureAwait(false);

                if (!IgnoreTransaction)
                    await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                if (!IgnoreTransaction)
                    await transaction.RollbackAsync();

                _workItem = () =>
                {
                    //TODO:Add Details here
                    return new FAILED_TRANSACTIONS_LOG();
                };

                await BackGroundTaskQueue.QueueBackGroundWorKAsync(BuildBackGroundWorkForFailedTransactions);
                throw new ReturnStatusException(StatusCode.Cancelled, HandleException(ex));
            }
        }   
     }

    /// <summary>
    ///     Exception Handling
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    private  string HandleException(Exception ex)
    {
        Logger.Log(LogLevel.Error, ex.Message);

        return DbExceptionHandler.HandleException(ex);
    }
}


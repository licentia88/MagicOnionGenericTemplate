using Grpc.Core;
using MagicOnion;
using MagicOnion.Server.Hubs;
using MagicT.Server.Exceptions;
using MagicT.Shared.Hubs.Base;
using MagicT.Shared.Models.ServiceModels;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Hubs.Base;

public partial class MagicHubServerBase<THub, TReceiver, TModel, TContext> : StreamingHubBase<THub, TReceiver>,
    IMagicTHub<THub, TReceiver, TModel>
    where THub : IStreamingHub<THub, TReceiver>
    where TReceiver : IMagicTReceiver<TModel>
    where TContext : DbContext
    where TModel : class, new()
{
    /// <summary>
    ///     Executes an asynchronous task and wraps the result in a <see cref="RESPONSE_RESULT{TModel}" /> object.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The asynchronous task to execute.</param>
    /// <returns>A <see cref="UnaryResult{T}" /> containing the result of the task.</returns>
    public async UnaryResult<RESPONSE_RESULT<T>> ExecuteAsync<T>(Func<Task<T>> task) where T : new()
    {
        try
        {
            var result = await task().ConfigureAwait(false);
            return new RESPONSE_RESULT<T>(result);
        }
        catch (Exception ex)
        {

            throw new ReturnStatusException(StatusCode.Cancelled, HandleException(ex));
        }
    }

    /// <summary>
    ///     Executes a synchronous task and wraps the result in a <see cref="RESPONSE_RESULT{T}" /> object.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The synchronous task to execute.</param>
    /// <returns>A <see cref="UnaryResult{T}" /> containing the result of the task.</returns>
    public UnaryResult<RESPONSE_RESULT<T>> Execute<T>(Func<T> task) where T : new()
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
    public async UnaryResult<T> ExecuteAsyncWithoutResponse<T>(Func<Task<T>> task)
    {
        try
        {
            var result = await task().ConfigureAwait(false);
            return result;
        }
        catch (Exception ex)
        {
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

    /// <summary>
    ///     Exception Handling
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    private string HandleException(Exception ex)
    {

        return DbExceptionHandler.HandleException(ex);
    }
}


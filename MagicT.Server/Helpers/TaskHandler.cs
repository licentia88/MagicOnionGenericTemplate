using Grpc.Core;
using MagicOnion;
using MagicT.Server.Exceptions;
using MagicT.Shared.Models.ServiceModels;

namespace MagicT.Server.Helpers;

/// <summary>
/// A utility class that handles exception handling and execution of tasks.
/// </summary>
public static class TaskHandler
{
    private static readonly DbExceptionHandler ExceptionHandler = new();


    /// <summary>
    /// Executes an asynchronous task and wraps the result in a <see cref="RESPONSE_RESULT{TModel}"/> object.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The asynchronous task to execute.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the result of the task.</returns>
    public static async UnaryResult<RESPONSE_RESULT<T>> ExecuteAsync<T>(Func<Task<T>> task) where T : new()
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
    /// Executes a synchronous task and wraps the result in a <see cref="RESPONSE_RESULT{T}"/> object.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The synchronous task to execute.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the result of the task.</returns>
    public static UnaryResult<RESPONSE_RESULT<T>> Execute<T>(Func<T> task) where T : new()
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
    /// Executes an asynchronous task without returning a response.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The asynchronous task to execute.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the result of the task.</returns>
    public static async UnaryResult<T> ExecuteAsyncWithoutResponse<T>(Func<Task<T>> task)
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
    /// Executes a synchronous task without returning a response.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The synchronous task to execute.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the result of the task.</returns>
    public static UnaryResult<T> ExecuteWithoutResponse<T>(Func<T> task)
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
    /// Executes an action.
    /// </summary>
    /// <param name="task">The action to execute.</param>
    public static void Execute(Action task)
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
    public static async Task ExecuteAsync(Func<Task> task)
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
    /// Exception Handling
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    private static string HandleException(Exception ex)
    {
        return ExceptionHandler.HandleException(ex);
    }

    /// <summary>
    /// Executes an asynchronous task and performs an action on its result.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="result">The asynchronous task result to act upon.</param>
    /// <param name="action">The action to perform on the result.</param>
    /// <returns>The original task result after the action is performed.</returns>
    public static async Task<T> OnComplete<T>(this UnaryResult<T> result, Func<T, Task> action)
    {
        var data = await result;
        await action.Invoke(data);
        return data;
    }

    /// <summary>
    /// Executes an asynchronous task and performs an action on its result.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="result">The asynchronous task result to act upon.</param>
    /// <param name="action">The action to perform on the result.</param>
    /// <returns>The original task result after the action is performed.</returns>
    public static async Task<T> OnComplete<T>(this UnaryResult<T> result, Action<T> action)
    {
        var data = await result;
        action.Invoke(data);
        return data;
    }

    /// <summary>
    /// Executes an asynchronous task and performs an action on its result with an additional argument.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <typeparam name="TArg">The type of the additional argument for the action.</typeparam>
    /// <param name="result">The asynchronous task result to act upon.</param>
    /// <param name="action">The action to perform on the result.</param>
    /// <param name="arg">The additional argument for the action.</param>
    /// <returns>The original task result after the action is performed.</returns>
    public static async Task<T> OnComplete<T, TArg>(this UnaryResult<T> result, Action<T, TArg> action, TArg arg)
    {
        var data = await result;
        action.Invoke(data, arg);
        return data;
    }

    /// <summary>
    /// Executes an asynchronous task and performs an action after its completion.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="result">The asynchronous task result to act upon.</param>
    /// <param name="action">The action to perform after the task completion.</param>
    /// <returns>The original task result after the action is performed.</returns>
    public static async Task<T> OnComplete<T>(this UnaryResult<T> result, Action action)
    {
        var data = await result;
        action.Invoke();
        return data;
    }

 

}
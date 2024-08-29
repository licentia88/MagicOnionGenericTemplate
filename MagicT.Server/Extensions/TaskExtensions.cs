using MagicOnion;
using MagicT.Shared.Enums;

namespace MagicT.Server.Extensions;
 
/// <summary>
/// Extension methods for working with asynchronous operations represented by UnaryResult.
/// </summary>
public static class TaskExtensions
{
    public static async Task OnComplete(this Task task, Action<TaskResult> action)
    {
        TaskResult status = TaskResult.Success;
 
        try
        {
            await task;

            action.Invoke( status);

        }
        catch(Exception ex)
        {
            status = TaskResult.Fail;

            action.Invoke(status);

            throw;
        }
        
    }

     
    public static async Task OnComplete(this Task task, Action<TaskResult, Exception> action)
    {
        TaskResult status = TaskResult.Success;
        
        try
        {
            await task;

            action.Invoke(status,null);

        }
        catch (Exception ex)
        {
            status = TaskResult.Fail;

            action.Invoke( status,ex);

            throw;
        }
        
    }

    
    
    /// <summary>
    /// Executes an asynchronous action before and after the completion of the task.
    /// </summary>
    /// <typeparam name="T">The type of data returned by the task.</typeparam>
    /// <param name="task">The asynchronous task to be monitored.</param>
    /// <param name="func">A function that takes the result of the task and its completion status and returns a new task result.</param>
    /// <returns>The result of the original task.</returns>
    public static async UnaryResult<T> OnComplete<T>(this UnaryResult<T> task, Func<T, TaskResult, UnaryResult<T>> func) where T:class
    {
        TaskResult status = TaskResult.Success;
        
        T data = null;

        try
        {
            data = await task;

            data = await func.Invoke(data, status);
        }
        catch(Exception ex)
        {
            status = TaskResult.Fail;

            await func.Invoke(data, status);

            throw;
        }

        return data;

    }
    

    /// <summary>
    /// Executes an action before and after the completion of the task, without handling the result data.
    /// </summary>
    /// <typeparam name="T">The type of data returned by the task.</typeparam>
    /// <param name="task">The asynchronous task to be monitored.</param>
    /// <param name="action">An action that takes the completion status of the task.</param>
    /// <returns>The result of the original task.</returns>
    public static async UnaryResult<T> OnComplete<T>(this UnaryResult<T> task, Action<TaskResult> action) where T:class
    {
        TaskResult status = TaskResult.Success;
        T data;
        try
        {
            data = await task;

            action.Invoke(status);
        }
        catch(Exception ex)
        {
            status = TaskResult.Fail;

            action.Invoke(status);

            throw;
        }
 
        return data;
    }

    /// <summary>
    /// Executes an action before and after the completion of the task.
    /// </summary>
    /// <typeparam name="T">The type of data returned by the task.</typeparam>
    /// <param name="task">The asynchronous task to be monitored.</param>
    /// <param name="action">An action that takes the result of the task and its completion status.</param>
    /// <returns>The result of the original task.</returns>
    public static async UnaryResult<T> OnComplete<T>(this UnaryResult<T> task, Action<T, TaskResult> action)
    {
        TaskResult status = TaskResult.Success;
        T data = default;

        try
        {
            data = await task;

            action.Invoke(data, status);

        }
        catch(Exception ex)
        {
            status = TaskResult.Fail;

            action.Invoke(data, status);

            throw;
        }

        return data;

    }

    /// <summary>
    /// Executes an action before and after the completion of the task.
    /// </summary>
    /// <typeparam name="T">The type of data returned by the task.</typeparam>
    /// <param name="task">The asynchronous task to be monitored.</param>
    /// <param name="action">An action that takes the result of the task and its completion status.</param>
    /// <returns>The result of the original task.</returns>
    public static async UnaryResult<T> OnComplete<T>(this UnaryResult<T> task, Action<T, TaskResult, Exception> action)
    {
        TaskResult status = TaskResult.Success;
        
        T data = default;

        try
        {
            data = await task;

            action.Invoke(data, status,null);

        }
        catch (Exception ex)
        {
            status = TaskResult.Fail;

            action.Invoke(data, status,ex);

            throw;
        }

        return data;

    }
     
    /// <summary>
    /// Executes an action with an additional argument before and after the completion of the task.
    /// </summary>
    /// <typeparam name="T">The type of data returned by the task.</typeparam>
    /// <typeparam name="TArg">The type of the additional argument for the action.</typeparam>
    /// <param name="task">The asynchronous task to be monitored.</param>
    /// <param name="action">An action that takes the result of the task, its completion status, and an additional argument.</param>
    /// <param name="arg">The additional argument to be passed to the action.</param>
    /// <returns>The result of the original task.</returns>
    public static async UnaryResult<T> OnComplete<T, TArg>(this UnaryResult<T> task, Action<T, TaskResult, TArg> action, TArg arg) where T:class
    {
        TaskResult status = TaskResult.Success;
        T data = null;

        try
        {
            data = await task;

            action.Invoke(data, status, arg);
        }
        catch(Exception ex)
        {
            status = TaskResult.Fail;

            action.Invoke(data, status, arg);

            throw;
        }

        return data;
    }

 }

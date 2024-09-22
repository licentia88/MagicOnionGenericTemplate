using MagicT.Shared.Enums;
using MessagePipe;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Web.Shared.Extensions;

 

/// <summary>
/// Extension methods for working with asynchronous operations represented by Task<>.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Executes an asynchronous action before and after the completion of the task.
    /// </summary>
    /// <typeparam name="T">The type of data returned by the task.</typeparam>
    /// <param name="task">The asynchronous task to be monitored.</param>
    /// <param name="func">A function that takes the result of the task and its completion status and returns a new task result.</param>
    /// <returns>The result of the original task.</returns>
    public static async Task<T> OnComplete<T>(this Task<T> task, Func<T, TaskResult, Task<T>> func) where T : class
    {
        TaskResult _status = TaskResult.Success;
        T _data = null;

        try
        {
            _data = await task;

            if (_data is null)
                _status = TaskResult.Fail;

            _data = await func.Invoke(_data, _status);
        }
        catch (Exception ex)
        {
            _status = TaskResult.Fail;
            await func.Invoke(_data, _status);
        }

        return _data;
    }

    /// <summary>
    /// Executes an action before and after the completion of the task.
    /// </summary>
    /// <typeparam name="T">The type of data returned by the task.</typeparam>
    /// <param name="task">The asynchronous task to be monitored.</param>
    /// <param name="action">An action that takes the result of the task and its completion status.</param>
    /// <returns>The result of the original task.</returns>
    public static async Task<T> OnComplete<T>(this Task<T> task, Action<T, TaskResult> action) where T : class
    {
        TaskResult _status = TaskResult.Success;
        T _data = null;

        try
        {
            _data = await task;

            if (_data is null)
                _status = TaskResult.Fail;

            action.Invoke(_data, _status);

        }
        catch
        {
            _status = TaskResult.Fail;
            action.Invoke(_data, _status);
        }


        return _data;
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
    public static async Task<T> OnComplete<T, TArg>(this Task<T> task, Action<T, TaskResult, TArg> action, TArg arg) where T : class
    {
        TaskResult _status = TaskResult.Success;
        T _data = null;

        try
        {
            _data = await task;

            if (_data is null)
                _status = TaskResult.Fail;

            action.Invoke(_data, _status, arg);

        }
        catch
        {
            _status = TaskResult.Fail;
            action.Invoke(_data, _status, arg);
        }


        return _data;
    }

    /// <summary>
    /// Executes an action before and after the completion of the task, without handling the result data.
    /// </summary>
    /// <typeparam name="T">The type of data returned by the task.</typeparam>
    /// <param name="task">The asynchronous task to be monitored.</param>
    /// <param name="action">An action that takes the completion status of the task.</param>
    /// <returns>The result of the original task.</returns>
    public static async Task<T> OnComplete<T>(this Task<T> task, Action<TaskResult> action) where T : class
    {
        TaskResult _status = TaskResult.Success;
        T _data = default;
        try
        {
            _data = await task;

            if (_data is null)
                _status = TaskResult.Fail;

            action.Invoke(_status);

        }
        catch
        {
            _status = TaskResult.Fail;
            action.Invoke(_status);
        }


        return _data;
    }

}


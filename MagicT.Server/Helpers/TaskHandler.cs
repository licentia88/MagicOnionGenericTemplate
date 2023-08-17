using Grpc.Core;
using MagicOnion;
using MagicT.Server.Exceptions;
using MagicT.Shared.Models.ServiceModels;

namespace MagicT.Server.Helpers;

/// <summary>
///     A utility class that handles exception handling and execution of tasks.
/// </summary>
public static class TaskHandler
{
   

    /// <summary>
    ///     Executes an asynchronous task and performs an action on its result.
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
    ///     Executes an asynchronous task and performs an action on its result.
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
    ///     Executes an asynchronous task and performs an action on its result with an additional argument.
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
    ///     Executes an asynchronous task and performs an action after its completion.
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
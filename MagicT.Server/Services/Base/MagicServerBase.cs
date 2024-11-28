using System.Runtime.CompilerServices;
using Benutomo;
using Coravel.Queuing.Interfaces;
using EntityFramework.Exceptions.Common;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using MagicT.Redis;
using MagicT.Server.Extensions;
using MagicT.Server.Jwt;
using MagicT.Server.Models;
using MagicT.Shared.Managers;

namespace MagicT.Server.Services.Base;

/// <summary>
/// Base class for MagicT server services providing common functionality.
/// </summary>
/// <typeparam name="TService">The service interface.</typeparam>
[AutomaticDisposeImpl]
public abstract partial class MagicServerBase<TService> : ServiceBase<TService>, IDisposable,IAsyncDisposable where TService : IService<TService>
{
    /// <summary>
    /// The queue instance used for queuing tasks.
    /// </summary>
    protected readonly IQueue Queue;

    /// <summary>
    /// The Redis database instance used for accsessing Redis data.
    /// </summary>
    [EnableAutomaticDispose]
    protected MagicTRedisDatabase MagicTRedisDatabase { get; set; }

    /// <summary>
    /// The cancellation token manager instance used for managing cancellation tokens.
    /// </summary>
    [EnableAutomaticDispose]
    protected CancellationTokenManager CancellationTokenManager { get; set; }

    /// <summary>
    /// Gets the current user's token.
    /// </summary>
    [EnableAutomaticDispose]
    private MagicTToken Token => Context.GetItemAs<MagicTToken>(nameof(MagicTToken));

    /// <summary>
    /// Gets the current user's ID.
    /// </summary>
    protected int CurrentUserId => Token?.Id ?? 0;

    /// <summary>
    /// Gets the shared key for the current user.
    /// </summary>
    protected byte[] SharedKey => MagicTRedisDatabase.ReadAs<UsersCredentials>(CurrentUserId.ToString()).SharedKey;

    /// <summary>
    /// The log manager instance used for logging operations.
    /// </summary>
    [EnableAutomaticDispose]
    protected LogManager<TService> LogManager { get; set; }

   
    /// <summary>
    /// Initializes a new instance of the <see cref="MagicServerBase{TService}"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    protected MagicServerBase(IServiceProvider provider)
    {
        Queue = provider.GetService<IQueue>();
        MagicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();
        CancellationTokenManager = provider.GetService<CancellationTokenManager>();
        LogManager = provider.GetService<LogManager<TService>>();
    }

    ~MagicServerBase()
    {
        Dispose(false);
    }
    /// <summary>
    /// Executes an asynchronous task and returns the result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="task">The asynchronous task to execute.</param>
    /// <param name="callerFilePath">The source file path of the caller.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    /// <param name="callerLineNumber">The line number in the source file at which the method is called.</param>
    /// <returns>The result of the task.</returns>
    protected virtual async UnaryResult<T> ExecuteAsync<T>(Func<Task<T>> task,
        [CallerFilePath] string callerFilePath = default,
        [CallerMemberName] string callerMemberName = default,
        [CallerLineNumber] int callerLineNumber = default)
    {
        try
        {
            var result = await task();
            LogManager.LogMessage(CurrentUserId, "", callerFilePath, callerMemberName);
            return result;
        }
        catch (Exception ex)
        {
            HandleError(ex, callerFilePath, callerMemberName, callerLineNumber);
            return default;
        }
    }

    /// <summary>
    /// Executes a synchronous task and returns the result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="task">The synchronous task to execute.</param>
    /// <param name="callerFilePath">The source file path of the caller.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    /// <param name="callerLineNumber">The line number in the source file at which the method is called.</param>
    /// <returns>The result of the task.</returns>
    protected virtual UnaryResult<T> ExecuteAsync<T>(Func<T> task, [CallerFilePath] string callerFilePath = default,
        [CallerMemberName] string callerMemberName = default,
        [CallerLineNumber] int callerLineNumber = default)
    {
        try
        {
            var result = task();
            LogManager.LogMessage(CurrentUserId, "", callerFilePath, callerMemberName);
            return UnaryResult.FromResult(result);
        }
        catch (Exception ex)
        {
            HandleError(ex, callerFilePath, callerMemberName, callerLineNumber);
            return default;
        }
    }

    /// <summary>
    /// Executes an asynchronous task without returning a response.
    /// </summary>
    /// <param name="task">The asynchronous task to execute.</param>
    /// <param name="callerFilePath">The source file path of the caller.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    /// <param name="callerLineNumber">The line number in the source file at which the method is called.</param>
    protected virtual async Task ExecuteAsync(Func<Task> task, [CallerFilePath] string callerFilePath = default,
        [CallerMemberName] string callerMemberName = default,
        [CallerLineNumber] int callerLineNumber = default)
    {
        try
        {
            await task();
            LogManager.LogMessage(CurrentUserId, "", callerFilePath, callerMemberName);
        }
        catch (Exception ex)
        {
            HandleError(ex, callerFilePath, callerMemberName, callerLineNumber);
        }
    }

    /// <summary>
    /// Executes an action.
    /// </summary>
    /// <param name="task">The action to execute.</param>
    /// <param name="callerFilePath">The source file path of the caller.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    /// <param name="callerLineNumber">The line number in the source file at which the method is called.</param>
    /// <param name="message">An optional message that provides additional context for the execution.</param>
    protected virtual void Execute(Action task, [CallerFilePath] string callerFilePath = default,
        [CallerMemberName] string callerMemberName = default,
        [CallerLineNumber] int callerLineNumber = default, string message = default)
    {
        try
        {
            task();
            LogManager.LogMessage(CurrentUserId, message, callerFilePath, callerMemberName);
        }
        catch (Exception ex)
        {
            HandleError(ex, callerFilePath, callerMemberName, callerLineNumber);
        }
    }

    /// <summary>
    /// Handles database exceptions.
    /// </summary>
    /// <param name="ex">The exception that occurred.</param>
    /// <param name="callerFilePath">The source file path of the caller.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    /// <param name="callerLineNumber">The line number in the source file at which the method is called.</param>
    /// <exception cref="ReturnStatusException">Thrown when a database exception occurs.</exception>
    protected virtual void HandleError(Exception ex, string callerFilePath, string callerMemberName, int callerLineNumber)
    {
        string errorMessage = ex switch
        {
            UniqueConstraintException uniqueEx => $"A unique constraint violation occurred: {uniqueEx.Message}",
            ReferenceConstraintException referenceEx => $"A reference constraint violation occurred: {referenceEx.Message}",
            CannotInsertNullException nullEx => $"A not null constraint violation occurred: {nullEx.Message}",
            MaxLengthExceededException lengthEx => $"A max length constraint violation occurred: {lengthEx.Message}",
            DbUpdateException updateException => $"{updateException.InnerException?.Message}",
            ReturnStatusException returnStatusException => returnStatusException.Detail,
            _ => ex.Message
        };

        LogManager.LogError(CurrentUserId, errorMessage, callerFilePath, callerMemberName, callerLineNumber,GetType().Name);
        throw new ReturnStatusException(StatusCode.Cancelled, errorMessage);
    }
}
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
using MagicT.Server.Managers;
using MagicT.Server.Models;
using MagicT.Shared.Managers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MagicT.Server.Services.Base;

[AutomaticDisposeImpl]
public abstract partial class MagicServerBase<TService> : ServiceBase<TService> ,IDisposable,IAsyncDisposable where TService : IService<TService>
{
    protected readonly IQueue Queue;
    
    [EnableAutomaticDispose]
    protected MagicTRedisDatabase MagicTRedisDatabase { get; set; }

    [EnableAutomaticDispose] private CancellationTokenManager CancellationTokenManager { get; set; }

    private MagicTToken Token => Context.GetItemAs<MagicTToken>(nameof(MagicTToken));

    protected int CurrentUserId => Token?.Id ?? 0;
    
    protected byte[] SharedKey => MagicTRedisDatabase.ReadAs<UsersCredentials>(CurrentUserId.ToString()).SharedKey;

   
    protected LogManager<TService> LogManager { get; set; }

    // private AsyncSemaphore Semaphore { get; set; }


    protected MagicServerBase(IServiceProvider provider)
    {
        Queue = provider.GetService<IQueue>();

        MagicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();

        CancellationTokenManager = provider.GetService<CancellationTokenManager>();

        LogManager = provider.GetService<LogManager<TService>>();

        // Semaphore = provider.GetService<AsyncSemaphore>();

    }
    
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


    

    protected virtual  UnaryResult<T> ExecuteAsync<T>(Func<T> task, [CallerFilePath] string callerFilePath = default,
        [CallerMemberName] string callerMemberName = default,
        [CallerLineNumber] int callerLineNumber = default)
    {
        try
        {
            // Semaphore.Wait();
            var result =  task();
 
            LogManager.LogMessage(CurrentUserId,"",callerFilePath,callerMemberName);

            // Semaphore.Release();
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
    /// <param name="callerFilePath"></param>
    /// <param name="callerMemberName"></param>
    /// <param name="callerLineNumber"></param>
    protected virtual async Task ExecuteAsync(Func<Task> task, [CallerFilePath] string callerFilePath = default,
        [CallerMemberName] string callerMemberName = default,
        [CallerLineNumber] int callerLineNumber = default)
    {

        try
        {
            await task();
 
            LogManager.LogMessage(CurrentUserId,"",callerFilePath,callerMemberName);

        }
        catch (Exception ex)
        {
            HandleError(ex, callerFilePath, callerMemberName, callerLineNumber);
        }

    }
    
    /// <summary>
    ///     Executes an action.
    /// </summary>
    /// <param name="task">The action to execute.</param>
    /// <param name="callerFilePath"></param>
    /// <param name="callerMemberName"></param>
    /// <param name="callerLineNumber"></param>
    protected virtual void Execute(Action task, [CallerFilePath] string callerFilePath = default,
        [CallerMemberName] string callerMemberName = default,
        [CallerLineNumber] int callerLineNumber = default)
    {
        try
        {
            task();
 
            LogManager.LogMessage(CurrentUserId,"",callerFilePath,callerMemberName);
        }
        catch (Exception ex)
        {
            HandleError(ex, callerFilePath, callerMemberName, callerLineNumber);
        }
    }
    protected virtual void HandleError(Exception ex, string callerFilePath, string callerMemberName, int callerLineNumber)
    {
        string errorMessage = ex switch
        {
            UniqueConstraintException uniqueEx => $"A unique constraint violation occurred: {uniqueEx.Message}",
            ReferenceConstraintException referenceEx => $"A reference constraint violation occurred: {referenceEx.Message}",
            CannotInsertNullException nullEx => $"A not null constraint violation occurred: {nullEx.Message}",
            MaxLengthExceededException lengthEx => $"A max length constraint violation occurred: {lengthEx.Message}",
            DbUpdateException updateException => $"{updateException.InnerException?.Message}",
            _ => ex.Message
        };

        LogManager.LogError(CurrentUserId, errorMessage, callerFilePath, callerMemberName, callerLineNumber);

        throw new ReturnStatusException(StatusCode.Cancelled, errorMessage);
    }

 
}
